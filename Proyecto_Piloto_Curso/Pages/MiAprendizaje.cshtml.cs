using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Proyecto_Piloto_Curso.Datos;
using Proyecto_Piloto_Curso.Modelos;
using Proyecto_Piloto_Curso.Services; // Inclusión necesaria

namespace Proyecto_Piloto_Curso.Pages
{
    public class MiAprendizajeModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly PdfService _pdfService;

        // 1. ESTAS SON LAS PROPIEDADES (Pégalas justo aquí, arriba de CursosInscritos)
        public int PaginaActual { get; set; }
        public int TotalPaginas { get; set; }
        public List<CursoInscrito> CursosInscritos { get; set; } = new List<CursoInscrito>();

        [TempData]
        public string Mensaje { get; set; } = "";

        public MiAprendizajeModel(ApplicationDbContext context, PdfService pdfService)
        {
            _context = context;
            _pdfService = pdfService;
        }

        // 2. ESTE ES EL MÉTODO ONGET (Reemplaza TODO tu OnGetAsync anterior por este)
        public async Task OnGetAsync(int p = 1)
        {
            var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            if (!usuarioId.HasValue) return;

            int registrosPorPagina = 4;
            PaginaActual = p;

            var totalRegistros = await _context.Inscripciones
                .Where(i => i.usuario_id == usuarioId.Value)
                .CountAsync();

            TotalPaginas = (int)Math.Ceiling(totalRegistros / (double)registrosPorPagina);

            var inscripciones = await _context.Inscripciones
                .Include(i => i.CursoNavigation)
                    .ThenInclude(c => c.CategoriaNavigation)
                .Where(i => i.usuario_id == usuarioId.Value)
                .OrderByDescending(i => i.fecha_inscripcion)
                .Skip((p - 1) * registrosPorPagina)
                .Take(registrosPorPagina)
                .ToListAsync();

            CursosInscritos.Clear();
            foreach (var ins in inscripciones)
            {
                CursosInscritos.Add(new CursoInscrito
                {
                    Curso = ins.CursoNavigation!,
                    Progreso = ins.progreso,
                    FechaInscripcion = ins.fecha_inscripcion
                });
            }
        }

        // NUEVO: Acción para previsualizar y descargar el PDF
        public async Task<IActionResult> OnGetExportarPdf()
        {
            var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            if (!usuarioId.HasValue) return RedirectToPage("/Login");

            var estudiante = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.idUsuarios == usuarioId.Value);

            if (estudiante == null) return RedirectToPage("/Login");

            var inscripciones = await _context.Inscripciones
                .Include(i => i.CursoNavigation)
                    .ThenInclude(c => c.CategoriaNavigation)
                .Include(i => i.CursoNavigation)
                    .ThenInclude(c => c.DocenteNavigation)
                        .ThenInclude(d => d.UsuarioNavigation)
                .Where(i => i.usuario_id == usuarioId.Value)
                .ToListAsync();

            var pdfBytes = _pdfService.GenerarReporteAprendizaje(estudiante, inscripciones);

            // IMPORTANTE: Al quitar el nombre del archivo "MiAprendizaje.pdf", 
            // el navegador lo abrirá en una pestaña nueva para previsualizar.
            return File(pdfBytes, "application/pdf");
        }

        public async Task<IActionResult> OnPostCancelarInscripcion(int cursoId)
        {
            var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            if (!usuarioId.HasValue) return RedirectToPage("/Login");

            var inscripcion = await _context.Inscripciones
                .FirstOrDefaultAsync(i => i.usuario_id == usuarioId && i.curso_id == cursoId);

            if (inscripcion != null)
            {
                _context.Inscripciones.Remove(inscripcion);
                await _context.SaveChangesAsync();
                Mensaje = "Se ha cancelado tu inscripción al curso.";
            }

            return RedirectToPage();
        }
    }

    public class CursoInscrito
    {
        public Curso Curso { get; set; } = new Curso();
        public decimal Progreso { get; set; }
        public DateTime FechaInscripcion { get; set; }
    }


}