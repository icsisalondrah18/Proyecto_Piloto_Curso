using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Proyecto_Piloto_Curso.Datos;
using Proyecto_Piloto_Curso.Modelos;

namespace Proyecto_Piloto_Curso.Pages.Admin
{
    public class EditarCursoModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        [BindProperty]
        public Curso Curso { get; set; } = new Curso();

        public SelectList? CategoriasList { get; set; }
        public SelectList? DocentesList { get; set; }

        public string MensajeExito { get; set; } = string.Empty;

        public EditarCursoModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            await CargarListas();

            Curso = await _context.Cursos
                .FirstOrDefaultAsync(c => c.idCursos == id) ?? new Curso();

            if (Curso.idCursos == 0) return NotFound();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await CargarListas();
                return Page();
            }

            try
            {
                var cursoExistente = await _context.Cursos.FindAsync(Curso.idCursos);

                if (cursoExistente == null)
                {
                    return NotFound();
                }

                cursoExistente.titulo = Curso.titulo;
                cursoExistente.descripcion = Curso.descripcion;
                cursoExistente.precio = Curso.precio;
                cursoExistente.categoria_id = Curso.categoria_id;
                cursoExistente.docenteId = Curso.docenteId;

                await _context.SaveChangesAsync();
                MensajeExito = "Curso actualizado exitosamente";
                await CargarListas();
                return Page();
            }
            catch (Exception ex)
            {
                MensajeExito = $"Error: {ex.Message}";
                await CargarListas();
                return Page();
            }
        }

        private async Task CargarListas()
        {
            var categorias = await _context.Categorias.OrderBy(c => c.nombre).ToListAsync();
            CategoriasList = new SelectList(categorias, "idCat", "nombre");

            var docentes = await _context.Docentes.Include(d => d.UsuarioNavigation).ToListAsync();
            var docentesList = docentes.Select(d => new
            {
                d.idDocente,
                Nombre = d.UsuarioNavigation != null && !string.IsNullOrEmpty(d.UsuarioNavigation.nombre)
                    ? d.UsuarioNavigation.nombre
                    : $"Docente {d.idDocente}"
            }).OrderBy(d => d.Nombre).ToList();

            DocentesList = new SelectList(docentesList, "idDocente", "Nombre");
        }
    }
}