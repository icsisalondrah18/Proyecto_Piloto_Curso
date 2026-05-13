using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Proyecto_Piloto_Curso.Datos;
using Proyecto_Piloto_Curso.Modelos;

namespace Proyecto_Piloto_Curso.Pages
{
    public class LoginModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        [BindProperty]
        public string Username { get; set; } = "";

        [BindProperty]
        public string Password { get; set; } = "";

        public string MensajeError { get; set; } = "";

        public LoginModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
            {
                MensajeError = "Por favor ingrese usuario y contraseña";
                return Page();
            }

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.username == Username);

            if (usuario == null)
            {
                MensajeError = "Usuario no encontrado";
                return Page();
            }

            if (usuario.contrasena != Password)
            {
                MensajeError = "Contraseña incorrecta";
                return Page();
            }

            // Guardar en sesión
            HttpContext.Session.SetInt32("UsuarioId", usuario.idUsuarios);
            HttpContext.Session.SetString("UsuarioNombre", usuario.nombre ?? "");
            HttpContext.Session.SetString("UsuarioRol", usuario.rol ?? "estudiante");
            HttpContext.Session.SetString("Username", usuario.username ?? "");

            // Redirigir según rol
            if (usuario.rol == "admin")
            {
                return RedirectToPage("/Admin/Dashboard");
            }

            return RedirectToPage("/Index");
        }

        public IActionResult OnGetLogout()
        {
            HttpContext.Session.Clear();
            return RedirectToPage("/Login");
        }
    }
}