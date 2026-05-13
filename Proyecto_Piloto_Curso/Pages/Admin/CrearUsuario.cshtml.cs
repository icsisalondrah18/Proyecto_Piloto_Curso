using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Proyecto_Piloto_Curso.Datos;
using Proyecto_Piloto_Curso.Modelos;

namespace Proyecto_Piloto_Curso.Pages.Admin
{
    public class CrearUsuarioModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        [BindProperty]
        public Usuario NuevoUsuario { get; set; }

        public string MensajeExito { get; set; }
        public string MensajeError { get; set; }

        public CrearUsuarioModel(ApplicationDbContext context)
        {
            _context = context;
            NuevoUsuario = new Usuario();
            MensajeExito = string.Empty;
            MensajeError = string.Empty;
        }

        public void OnGet()
        {
            NuevoUsuario.fecha_creacion = DateTime.Now;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var existe = await _context.Usuarios
                .AnyAsync(u => u.username == NuevoUsuario.username || u.correo == NuevoUsuario.correo);

            if (existe)
            {
                MensajeError = "El nombre de usuario o correo ya existe";
                return Page();
            }

            try
            {
                NuevoUsuario.fecha_creacion = DateTime.Now;
                _context.Usuarios.Add(NuevoUsuario);
                await _context.SaveChangesAsync();

                MensajeExito = "Usuario creado exitosamente";

                NuevoUsuario = new Usuario();
                NuevoUsuario.fecha_creacion = DateTime.Now;
                return Page();
            }
            catch (Exception ex)
            {
                MensajeError = $"Error: {ex.Message}";
                return Page();
            }
        }
    }
}