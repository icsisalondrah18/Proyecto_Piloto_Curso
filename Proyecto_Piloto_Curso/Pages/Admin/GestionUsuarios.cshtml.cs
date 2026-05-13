using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Proyecto_Piloto_Curso.Datos;
using Proyecto_Piloto_Curso.Modelos;

namespace Proyecto_Piloto_Curso.Pages.Admin
{
    public class GestionUsuariosModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public List<Usuario> Usuarios { get; set; }
        public string MensajeExito { get; set; }
        public string MensajeError { get; set; }
        public int PaginaActual { get; set; }
        public int TotalPaginas { get; set; }
        public int TotalRegistros { get; set; }
        public string BusquedaActual { get; set; }

        public GestionUsuariosModel(ApplicationDbContext context)
        {
            _context = context;
            Usuarios = new List<Usuario>();
            MensajeExito = string.Empty;
            MensajeError = string.Empty;
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

            var query = _context.Usuarios.AsQueryable();

            if (!string.IsNullOrEmpty(BusquedaActual))
            {
                query = query.Where(u => u.nombre.Contains(BusquedaActual) ||
                                         u.username.Contains(BusquedaActual) ||
                                         u.correo.Contains(BusquedaActual));
            }

            TotalRegistros = await query.CountAsync();
            TotalPaginas = (int)Math.Ceiling((double)TotalRegistros / registrosPorPagina);

            Usuarios = await query
                .OrderByDescending(u => u.idUsuarios)
                .Skip((PaginaActual - 1) * registrosPorPagina)
                .Take(registrosPorPagina)
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostEliminarAsync(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario != null)
            {
                _context.Usuarios.Remove(usuario);
                await _context.SaveChangesAsync();
                MensajeExito = "Usuario eliminado correctamente";
            }
            return RedirectToPage();
        }
    }
}