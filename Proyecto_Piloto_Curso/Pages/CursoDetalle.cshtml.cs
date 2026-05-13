using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Proyecto_Piloto_Curso.Datos;
using Proyecto_Piloto_Curso.Modelos;

namespace Proyecto_Piloto_Curso.Pages
{
    public class CursoDetalleModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public Curso Curso { get; set; } = new Curso();
        public List<ModuloCurso> Modulos { get; set; } = new List<ModuloCurso>();
        public List<Objetivo> Objetivos { get; set; } = new List<Objetivo>();
        public List<Resena> Resenas { get; set; } = new List<Resena>();
        public List<Tarjeta> Tarjetas { get; set; } = new List<Tarjeta>();
        public bool EstaInscrito { get; set; }
        public string NombreInstructor { get; set; } = string.Empty;
        public string AreaInstructor { get; set; } = string.Empty;
        public int CantidadInscritos { get; set; }
        public double PromedioCalificacion { get; set; }

        public CursoDetalleModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Curso = await _context.Cursos
                .Include(c => c.CategoriaNavigation)
                .Include(c => c.DocenteNavigation)
                    .ThenInclude(d => d != null ? d.UsuarioNavigation : null)
                .FirstOrDefaultAsync(c => c.idCursos == id);

            if (Curso == null)
            {
                return NotFound();
            }

            Modulos = await _context.ModulosCursos
                .Where(m => m.cursoId == id)
                .OrderBy(m => m.orden)
                .ToListAsync();

            Objetivos = await _context.Objetivos
                .Where(o => o.curso_id == id)
                .ToListAsync();

            var resenasQuery = await _context.Resenas
                .Include(r => r.UsuarioNavigation)
                .Where(r => r.curso_id == id)
                .OrderByDescending(r => r.fecha_creacion)
                .ToListAsync();

            if (resenasQuery != null)
            {
                Resenas = resenasQuery;
            }

            if (Curso.DocenteNavigation != null && Curso.DocenteNavigation.UsuarioNavigation != null)
            {
                NombreInstructor = Curso.DocenteNavigation.UsuarioNavigation.nombre ?? "Instructor";
                AreaInstructor = Curso.DocenteNavigation.area ?? "General";
            }

            CantidadInscritos = await _context.Inscripciones
                .CountAsync(i => i.curso_id == id);

            var calificaciones = await _context.Resenas
                .Where(r => r.curso_id == id)
                .Select(r => r.calificacion)
                .ToListAsync();

            PromedioCalificacion = calificaciones.Any() ? calificaciones.Average() : 0;

            var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            if (usuarioId.HasValue)
            {
                EstaInscrito = await _context.Inscripciones
                    .AnyAsync(i => i.curso_id == id && i.usuario_id == usuarioId.Value);

                Tarjetas = await _context.Tarjetas
                    .Where(t => t.usuario_id == usuarioId.Value)
                    .ToListAsync();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostInscribirseAsync(int cursoId)
        {
            var usuarioId = HttpContext.Session.GetInt32("UsuarioId");

            if (!usuarioId.HasValue)
            {
                return RedirectToPage("/Login");
            }

            var yaInscrito = await _context.Inscripciones
                .AnyAsync(i => i.curso_id == cursoId && i.usuario_id == usuarioId.Value);

            if (!yaInscrito)
            {
                var curso = await _context.Cursos.FindAsync(cursoId);

                if (curso != null && curso.precio == 0)
                {
                    await InscribirUsuario(cursoId, usuarioId.Value);
                    TempData["MensajeExito"] = "Te has inscrito exitosamente al curso";
                }
                else
                {
                    TempData["MensajeError"] = "Este curso requiere pago";
                }
            }

            return RedirectToPage(new { id = cursoId });
        }

        public async Task<IActionResult> OnPostProcesarPagoAsync(int cursoId, int tarjetaId, string cvv)
        {
            var usuarioId = HttpContext.Session.GetInt32("UsuarioId");

            if (!usuarioId.HasValue)
            {
                return RedirectToPage("/Login");
            }

            var curso = await _context.Cursos.FindAsync(cursoId);
            if (curso == null)
            {
                return NotFound();
            }

            var tarjeta = await _context.Tarjetas.FindAsync(tarjetaId);
            if (tarjeta == null || tarjeta.usuario_id != usuarioId.Value)
            {
                TempData["MensajeError"] = "Tarjeta no válida";
                return RedirectToPage(new { id = cursoId });
            }

            if (tarjeta.cvv != cvv)
            {
                TempData["MensajeError"] = "CVV incorrecto. Verifica el código de seguridad.";
                return RedirectToPage(new { id = cursoId });
            }

            if (!string.IsNullOrEmpty(tarjeta.fecha_expiracion))
            {
                var partes = tarjeta.fecha_expiracion.Split('/');
                if (partes.Length == 2)
                {
                    int mes = int.Parse(partes[0]);
                    int anio = int.Parse(partes[1]);
                    DateTime fechaExpiracion = new DateTime(2000 + anio, mes, 1).AddMonths(1).AddDays(-1);

                    if (fechaExpiracion < DateTime.Now)
                    {
                        TempData["MensajeError"] = "La tarjeta ha expirado. Usa otro método de pago.";
                        return RedirectToPage(new { id = cursoId });
                    }
                }
            }

            var maxIdPago = await _context.Pagos.MaxAsync(p => (int?)p.idPagos) ?? 0;
            var pago = new Pago
            {
                idPagos = maxIdPago + 1,
                usuario_id = usuarioId.Value,
                curso_id = cursoId,
                tarjeta_id = tarjetaId,
                precio_original = curso.precio,
                precio_final = curso.precio,
                fecha_creacion = DateTime.Now
            };
            _context.Pagos.Add(pago);

            await InscribirUsuario(cursoId, usuarioId.Value);
            await _context.SaveChangesAsync();

            TempData["MensajeExito"] = $"¡Pago exitoso! Te has inscrito al curso {curso.titulo}";

            return RedirectToPage(new { id = cursoId });
        }

        private async Task InscribirUsuario(int cursoId, int usuarioId)
        {
            var maxIdInscripcion = await _context.Inscripciones
                .MaxAsync(i => (int?)i.idInscripciones) ?? 0;

            var inscripcion = new Inscripcion
            {
                idInscripciones = maxIdInscripcion + 1,
                usuario_id = usuarioId,
                curso_id = cursoId,
                progreso = 0,
                fecha_inscripcion = DateTime.Now
            };

            _context.Inscripciones.Add(inscripcion);
            await _context.SaveChangesAsync();
        }
    }
}