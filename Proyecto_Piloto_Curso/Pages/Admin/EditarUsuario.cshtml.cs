using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Proyecto_Piloto_Curso.Datos;
using Proyecto_Piloto_Curso.Modelos;

namespace Proyecto_Piloto_Curso.Pages.Admin
{
    public class EditarUsuarioModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        [BindProperty]
        public Usuario Usuario { get; set; }

        public string MensajeExito { get; set; }
        public string MensajeError { get; set; }

        public EditarUsuarioModel(ApplicationDbContext context)
        {
            _context = context;
            Usuario = new Usuario();
            MensajeExito = string.Empty;
            MensajeError = string.Empty;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            if (id <= 0)
            {
                MensajeError = "ID de usuario no valido";
                return Page();
            }

            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                MensajeError = "Usuario no encontrado";
                return Page();
            }

            Usuario = usuario;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string nuevaContrasena)
        {
            if (Usuario == null || Usuario.idUsuarios == 0)
            {
                MensajeError = "Datos de usuario no validos";
                return Page();
            }

            var usuarioExistente = await _context.Usuarios.FindAsync(Usuario.idUsuarios);
            if (usuarioExistente == null)
            {
                MensajeError = "Usuario no encontrado";
                return Page();
            }

            usuarioExistente.username = Usuario.username;
            usuarioExistente.nombre = Usuario.nombre;
            usuarioExistente.correo = Usuario.correo;
            usuarioExistente.rol = Usuario.rol;

            if (!string.IsNullOrEmpty(nuevaContrasena))
            {
                usuarioExistente.contrasena = nuevaContrasena;
            }

            await _context.SaveChangesAsync();
            MensajeExito = "Usuario actualizado exitosamente";

            return Page();
        }
    }
}