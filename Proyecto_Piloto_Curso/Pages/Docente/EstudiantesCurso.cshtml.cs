using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Proyecto_Piloto_Curso.Datos;
using Proyecto_Piloto_Curso.Modelos;

namespace Proyecto_Piloto_Curso.Pages.Docente
{
    public class EstudiantesCursoModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public string CursoTitulo { get; set; } = string.Empty;
        public List<EstudianteInfo> Estudiantes { get; set; } = new List<EstudianteInfo>();
        public string UsuarioRol { get; set; } = string.Empty;

        public EstudiantesCursoModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task OnGetAsync(int id)
        {
            UsuarioRol = HttpContext.Session.GetString("UsuarioRol") ?? "";
            var usuarioId = HttpContext.Session.GetInt32("UsuarioId");

            // Verificar que el docente sea el dueño del curso o admin
            var curso = await _context.Cursos
                .FirstOrDefaultAsync(c => c.idCursos == id);

            if (curso == null)
                return;

            if (UsuarioRol != "admin" && curso.docenteId != usuarioId)
            {
                return;
            }

            CursoTitulo = curso.titulo ?? "";

            // Obtener inscripciones con datos de usuarios
            var inscripciones = await _context.Inscripciones
                .Include(i => i.UsuarioNavigation)
                .Where(i => i.curso_id == id)
                .ToListAsync();

            // Obtener reseñas del curso
            var resenas = await _context.Resenas
                .Where(r => r.curso_id == id)
                .ToListAsync();

            foreach (var ins in inscripciones)
            {
                var calificacion = resenas
                    .FirstOrDefault(r => r.usuario_id == ins.usuario_id)?.calificacion ?? 0;

                Estudiantes.Add(new EstudianteInfo
                {
                    NombreEstudiante = ins.UsuarioNavigation?.nombre ?? "Desconocido",
                    Correo = ins.UsuarioNavigation?.correo ?? "",
                    FechaInscripcion = ins.fecha_inscripcion,
                    Progreso = ins.progreso,
                    Calificacion = calificacion
                });
            }
        }
    }

    public class EstudianteInfo
    {
        public string NombreEstudiante { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public DateTime FechaInscripcion { get; set; }
        public decimal Progreso { get; set; }
        public int Calificacion { get; set; }
    }
}