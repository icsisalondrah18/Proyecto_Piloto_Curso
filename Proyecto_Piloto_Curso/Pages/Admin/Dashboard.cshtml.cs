using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Proyecto_Piloto_Curso.Datos;
using Proyecto_Piloto_Curso.Modelos;
using Proyecto_Piloto_Curso.Services;

namespace Proyecto_Piloto_Curso.Pages.Admin
{
    public class DashboardModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public int TotalCursos { get; set; }
        public int TotalUsuarios { get; set; }
        public int TotalInscripciones { get; set; }
        public int TotalDocentes { get; set; }
        public List<Curso> CursosRecientes { get; set; } = new List<Curso>();

        public DashboardModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task OnGetAsync()
        {
            TotalCursos = await _context.Cursos.CountAsync();
            TotalUsuarios = await _context.Usuarios.CountAsync();
            TotalInscripciones = await _context.Inscripciones.CountAsync();

            // ✅ Usar el nombre completo: Modelos.Docente
            TotalDocentes = await _context.Set<Modelos.Docente>().CountAsync();

            CursosRecientes = await _context.Cursos
                .OrderByDescending(c => c.idCursos)
                .Take(5)
                .ToListAsync();
        }

        // Reporte de Inscripciones
        public async Task<IActionResult> OnGetExportarInscripcionesPdfAsync()
        {
            var todasLasInscripciones = await _context.Inscripciones
                .Include(i => i.UsuarioNavigation)
                .Include(i => i.CursoNavigation)
                    .ThenInclude(c => c != null ? c.CategoriaNavigation : null)
                .Include(i => i.CursoNavigation)
                    .ThenInclude(c => c != null ? c.DocenteNavigation : null)
                        .ThenInclude(d => d != null ? d.UsuarioNavigation : null)
                .OrderByDescending(i => i.fecha_inscripcion)
                .ToListAsync();

            if (todasLasInscripciones == null)
            {
                todasLasInscripciones = new List<Inscripcion>();
            }

            var pdfService = new AdminPdfService();
            var pdfBytes = pdfService.GenerarReporteGeneralInscripciones(todasLasInscripciones);

            Response.Headers.Add("Content-Disposition", "inline; filename=Reporte_Inscripciones.pdf");
            return File(pdfBytes, "application/pdf");
        }

        // Reporte de Cursos
        public async Task<IActionResult> OnGetExportarCursosPdfAsync()
        {
            var todosLosCursos = await _context.Cursos
                .Include(c => c.CategoriaNavigation)
                .OrderBy(c => c.idCursos)
                .ToListAsync();

            if (todosLosCursos == null)
            {
                todosLosCursos = new List<Curso>();
            }

            var pdfService = new AdminPdfService();
            var pdfBytes = pdfService.GenerarReporteGeneralCursos(todosLosCursos);

            Response.Headers.Add("Content-Disposition", "inline; filename=Reporte_Cursos.pdf");
            return File(pdfBytes, "application/pdf");
        }

        // Reporte de Usuarios
        public async Task<IActionResult> OnGetExportarUsuariosPdfAsync()
        {
            var todosLosUsuarios = await _context.Usuarios.ToListAsync();

            if (todosLosUsuarios == null)
            {
                todosLosUsuarios = new List<Usuario>();
            }

            // ✅ Usar el nombre completo: Modelos.Docente
            var todosLosDocentes = await _context.Set<Modelos.Docente>()
                .Include(d => d.UsuarioNavigation)
                .ToListAsync();

            if (todosLosDocentes == null)
            {
                todosLosDocentes = new List<Modelos.Docente>();
            }

            var pdfService = new AdminPdfService();
            var pdfBytes = pdfService.GenerarReporteGeneralUsuarios(todosLosUsuarios, todosLosDocentes);

            Response.Headers.Add("Content-Disposition", "inline; filename=Reporte_Docente.pdf");
            return File(pdfBytes, "application/pdf");
        }
    }
}