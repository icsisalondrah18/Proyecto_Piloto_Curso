using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Proyecto_Piloto_Curso.Datos;
using Proyecto_Piloto_Curso.Modelos;

namespace Proyecto_Piloto_Curso.Pages.Admin
{
    public class CrearCursoModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        [BindProperty]
        public Curso Curso { get; set; } = new Curso();

        public SelectList? CategoriasList { get; set; }
        public SelectList? DocentesList { get; set; }

        public string MensajeExito { get; set; } = string.Empty;
        public string MensajeError { get; set; } = string.Empty;

        public CrearCursoModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            await CargarListas();

            // Obtener el siguiente ID disponible
            var ultimoId = await _context.Cursos.MaxAsync(c => (int?)c.idCursos) ?? 0;
            Curso.idCursos = ultimoId + 1;
            Curso.fecha_creacion = DateTime.Now;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await CargarListas();
                return Page();
            }

            try
            {
                // Verificar que el ID no exista (por si acaso)
                var existe = await _context.Cursos.AnyAsync(c => c.idCursos == Curso.idCursos);
                if (existe)
                {
                    var ultimoId = await _context.Cursos.MaxAsync(c => (int?)c.idCursos) ?? 0;
                    Curso.idCursos = ultimoId + 1;
                }

                _context.Cursos.Add(Curso);
                await _context.SaveChangesAsync();

                MensajeExito = $"Curso '{Curso.titulo}' creado exitosamente con ID {Curso.idCursos}";

                // Limpiar para otro nuevo curso
                Curso = new Curso();
                var nuevoUltimoId = await _context.Cursos.MaxAsync(c => (int?)c.idCursos) ?? 0;
                Curso.idCursos = nuevoUltimoId + 1;
                Curso.fecha_creacion = DateTime.Now;
                await CargarListas();

                return Page();
            }
            catch (DbUpdateException ex)
            {
                var innerMsg = ex.InnerException?.Message ?? ex.Message;
                MensajeError = $"Error: El ID {Curso.idCursos} puede que ya exista. {innerMsg}";
                await CargarListas();
                return Page();
            }
            catch (Exception ex)
            {
                MensajeError = $"Error: {ex.Message}";
                await CargarListas();
                return Page();
            }
        }

        private async Task CargarListas()
        {
            var categorias = await _context.Categorias.OrderBy(c => c.nombre).ToListAsync();
            CategoriasList = new SelectList(categorias, "idCat", "nombre");

            var docentes = await _context.Docentes.Include(d => d.UsuarioNavigation).ToListAsync();
            var docentesList = docentes.Select(d => new
            {
                d.idDocente,
                Nombre = d.UsuarioNavigation != null && !string.IsNullOrEmpty(d.UsuarioNavigation.nombre)
                    ? d.UsuarioNavigation.nombre
                    : $"Docente {d.idDocente}"
            }).OrderBy(d => d.Nombre).ToList();

            DocentesList = new SelectList(docentesList, "idDocente", "Nombre");
        }
    }
}