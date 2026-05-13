using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Proyecto_Piloto_Curso.Datos;
using Proyecto_Piloto_Curso.Modelos;

namespace Proyecto_Piloto_Curso.Pages
{
    public class RegistroModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        [BindProperty]
        public Usuario NuevoUsuario { get; set; } = new Usuario();

        public string MensajeExito { get; set; } = "";
        public string MensajeError { get; set; } = "";

        public RegistroModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Verificar si el usuario existe
            var existeUsuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.username == NuevoUsuario.username);

            if (existeUsuario != null)
            {
                MensajeError = "El nombre de usuario ya está en uso";
                return Page();
            }

            var existeCorreo = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.correo == NuevoUsuario.correo);

            if (existeCorreo != null)
            {
                MensajeError = "El correo electrónico ya está registrado";
                return Page();
            }

            // Asignar rol por defecto
            if (string.IsNullOrEmpty(NuevoUsuario.rol))
            {
                NuevoUsuario.rol = "estudiante";
            }

            NuevoUsuario.fecha_creacion = DateTime.Now;

            _context.Usuarios.Add(NuevoUsuario);
            await _context.SaveChangesAsync();

            MensajeExito = "¡Registro exitoso! Ahora puedes iniciar sesión.";
            return Page();
        }
    }
}