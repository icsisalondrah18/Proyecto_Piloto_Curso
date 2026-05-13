using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Proyecto_Piloto_Curso.Datos;
using Proyecto_Piloto_Curso.Modelos;

namespace Proyecto_Piloto_Curso.Pages.Admin
{
    public class GestionCursosModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public List<Curso> Cursos { get; set; }
        public string MensajeExito { get; set; }
        public int PaginaActual { get; set; }
        public int TotalPaginas { get; set; }
        public int TotalRegistros { get; set; }
        public string BusquedaActual { get; set; }

        public GestionCursosModel(ApplicationDbContext context)
        {
            _context = context;
            Cursos = new List<Curso>();
            MensajeExito = string.Empty;
            PaginaActual = 1;
            TotalPaginas = 1;
            TotalRegistros = 0;
            BusquedaActual = string.Empty;
        }

        public async Task OnGetAsync(int? pagina, string busqueda)
        {
            PaginaActual = pagina ?? 1;
            BusquedaActual = busqueda ?? string.Empty;
            int registrosPorPagina = 10;

            var query = _context.Cursos
                .Include(c => c.CategoriaNavigation)
                .Include(c => c.DocenteNavigation)
                    .ThenInclude(d => d != null ? d.UsuarioNavigation : null)
                .AsQueryable();

            if (!string.IsNullOrEmpty(BusquedaActual))
            {
                query = query.Where(c => c.titulo.Contains(BusquedaActual) ||
                    (c.CategoriaNavigation != null && c.CategoriaNavigation.nombre.Contains(BusquedaActual)));
            }

            TotalRegistros = await query.CountAsync();
            TotalPaginas = (int)Math.Ceiling((double)TotalRegistros / registrosPorPagina);

            Cursos = await query
                .OrderByDescending(c => c.idCursos)
                .Skip((PaginaActual - 1) * registrosPorPagina)
                .Take(registrosPorPagina)
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostEliminarAsync(int id)
        {
            var curso = await _context.Cursos.FindAsync(id);
            if (curso != null)
            {
                _context.Cursos.Remove(curso);
                await _context.SaveChangesAsync();
                MensajeExito = "Curso eliminado correctamente";
            }
            return RedirectToPage();
        }
    }
}