using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Proyecto_Piloto_Curso.Datos;
using Proyecto_Piloto_Curso.Modelos;
using Proyecto_Piloto_Curso.Helpers;

namespace Proyecto_Piloto_Curso.Pages
{
    public class CursosModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public List<Curso> CursosList { get; set; } = new List<Curso>();
        public List<Categoria> CategoriasList { get; set; } = new List<Categoria>();

        
        public int PaginaActual { get; set; } = 1;
        public int TotalPaginas { get; set; }
        public int TotalRegistros { get; set; }
        public int RegistrosPorPagina { get; set; } = 9;

        
        public int? CategoriaFiltro { get; set; }
        public string PrecioFiltro { get; set; } = "todos";

        public CursosModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task OnGetAsync(int? p, int? categoria, string precio)
        {
            PaginaActual = p ?? 1;
            CategoriaFiltro = categoria;
            PrecioFiltro = string.IsNullOrEmpty(precio) ? "todos" : precio;

           
            CategoriasList = await _context.Categorias
                .OrderBy(c => c.idCat)
                .ToListAsync();

            
            var query = _context.Cursos
                .Include(c => c.CategoriaNavigation)
                .Include(c => c.DocenteNavigation)
                    .ThenInclude(d => d != null ? d.UsuarioNavigation : null)
                .AsQueryable();

            
            if (CategoriaFiltro.HasValue && CategoriaFiltro.Value > 0)
            {
                query = query.Where(c => c.categoria_id == CategoriaFiltro.Value);
            }

            
            if (PrecioFiltro == "gratis")
            {
                query = query.Where(c => c.precio == 0);
            }
            else if (PrecioFiltro == "pago")
            {
                query = query.Where(c => c.precio > 0);
            }

            
            TotalRegistros = await query.CountAsync();
            TotalPaginas = (int)Math.Ceiling((double)TotalRegistros / RegistrosPorPagina);

            
            if (PaginaActual > TotalPaginas && TotalPaginas > 0)
            {
                PaginaActual = TotalPaginas;
            }
            if (PaginaActual < 1)
            {
                PaginaActual = 1;
            }

            
            CursosList = await query
                .OrderBy(c => c.idCursos)
                .Skip((PaginaActual - 1) * RegistrosPorPagina)
                .Take(RegistrosPorPagina)
                .ToListAsync();
        }
    }
}