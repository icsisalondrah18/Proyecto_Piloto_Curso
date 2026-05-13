using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Proyecto_Piloto_Curso.Datos;
using Proyecto_Piloto_Curso.Modelos;

namespace Proyecto_Piloto_Curso.Pages
{
    public class PerfilModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public Usuario? Usuario { get; set; }

        public PerfilModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task OnGetAsync()
        {
            var usuarioId = HttpContext.Session.GetInt32("UsuarioId");

            if (usuarioId.HasValue)
            {
                Usuario = await _context.Usuarios
                    .FirstOrDefaultAsync(u => u.idUsuarios == usuarioId.Value);
            }
        }
    }
}