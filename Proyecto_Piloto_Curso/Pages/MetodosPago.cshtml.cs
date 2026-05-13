using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Proyecto_Piloto_Curso.Datos;
using Proyecto_Piloto_Curso.Modelos;

namespace Proyecto_Piloto_Curso.Pages
{
    public class MetodosPagoModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public List<Tarjeta> Tarjetas { get; set; } = new List<Tarjeta>();
        public bool EstaLogueado { get; set; }

        public MetodosPagoModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task OnGetAsync()
        {
            var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            EstaLogueado = usuarioId.HasValue;

            if (usuarioId.HasValue)
            {
                Tarjetas = await _context.Tarjetas
                    .Where(t => t.usuario_id == usuarioId.Value)
                    .ToListAsync();
            }
        }

        public async Task<IActionResult> OnPostAgregarAsync(string tipo_tarjeta, string numero_tarjeta, string nombre_titular, string fecha_expiracion, string cvv, bool guardar_tarjeta)
        {
            var usuarioId = HttpContext.Session.GetInt32("UsuarioId");

            if (!usuarioId.HasValue)
                return RedirectToPage("/Login");

            string numeroLimpio = numero_tarjeta?.Replace(" ", "");

            // Validaciones
            if (tipo_tarjeta == "Amex")
            {
                if (numeroLimpio?.Length != 15)
                {
                    TempData["MensajeError"] = "American Express debe tener 15 dígitos";
                    return RedirectToPage();
                }
                if (cvv?.Length != 4)
                {
                    TempData["MensajeError"] = "American Express requiere CVV de 4 dígitos";
                    return RedirectToPage();
                }
            }
            else
            {
                if (numeroLimpio?.Length != 16)
                {
                    TempData["MensajeError"] = "Visa/Mastercard deben tener 16 dígitos";
                    return RedirectToPage();
                }
                if (cvv?.Length != 3)
                {
                    TempData["MensajeError"] = "CVV debe ser de 3 dígitos";
                    return RedirectToPage();
                }
            }

            var maxId = await _context.Tarjetas.MaxAsync(t => (int?)t.idTarjeta) ?? 0;

            var tarjeta = new Tarjeta
            {
                idTarjeta = maxId + 1,
                usuario_id = usuarioId.Value,
                tipo_tarjeta = tipo_tarjeta,
                numero_tarjeta = numeroLimpio,
                nombre_titular = nombre_titular?.ToUpper(),
                fecha_expiracion = fecha_expiracion,
                cvv = cvv  
            };

            _context.Tarjetas.Add(tarjeta);
            await _context.SaveChangesAsync();

            TempData["MensajeExito"] = $"Tarjeta {tipo_tarjeta} agregada exitosamente";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostEliminarAsync(int id)
        {
            var tarjeta = await _context.Tarjetas.FindAsync(id);
            if (tarjeta != null)
            {
                _context.Tarjetas.Remove(tarjeta);
                await _context.SaveChangesAsync();
                TempData["MensajeExito"] = "Tarjeta eliminada";
            }
            return RedirectToPage();
        }
    }
}