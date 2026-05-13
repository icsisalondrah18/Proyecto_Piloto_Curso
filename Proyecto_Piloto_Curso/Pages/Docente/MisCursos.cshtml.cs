using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Proyecto_Piloto_Curso.Datos;
using Proyecto_Piloto_Curso.Modelos;

namespace Proyecto_Piloto_Curso.Pages.Docente
{
    public class MisCursosModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public List<CursoInfo> Cursos { get; set; } = new List<CursoInfo>();
        public int TotalEstudiantes { get; set; }
        public int TotalModulos { get; set; }
        public double PromedioCalificaciones { get; set; }
        public string UsuarioNombre { get; set; } = string.Empty;
        public string UsuarioRol { get; set; } = string.Empty;

        public MisCursosModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task OnGetAsync()
        {
            // Obtener datos de sesión
            UsuarioNombre = HttpContext.Session.GetString("UsuarioNombre") ?? "";
            UsuarioRol = HttpContext.Session.GetString("UsuarioRol") ?? "";
            var usuarioId = HttpContext.Session.GetInt32("UsuarioId");

            if (!usuarioId.HasValue || (UsuarioRol != "docente" && UsuarioRol != "admin"))
            {
                return;
            }

            // Obtener cursos del docente
            var cursosDb = await _context.Cursos
                .Include(c => c.CategoriaNavigation)
                .Include(c => c.Modulos)
                .Include(c => c.Inscripciones)
                .ThenInclude(i => i.UsuarioNavigation)
                .Where(c => c.docenteId == usuarioId.Value)
                .ToListAsync();

            // Convertir a CursoInfo
            foreach (var curso in cursosDb)
            {
                var calificaciones = new List<int>();
                // Obtener calificaciones de las reseñas
                var resenas = await _context.Resenas
                    .Where(r => r.curso_id == curso.idCursos)
                    .ToListAsync();

                foreach (var res in resenas)
                {
                    calificaciones.Add(res.calificacion);
                }

                var promedio = calificaciones.Any() ? calificaciones.Average() : 0;

                Cursos.Add(new CursoInfo
                {
                    idCursos = curso.idCursos,
                    titulo = curso.titulo ?? "",
                    descripcion = curso.descripcion ?? "",
                    precio = curso.precio,
                    docenteId = curso.docenteId,
                    categoria_id = curso.categoria_id,
                    fecha_creacion = curso.fecha_creacion,
                    CategoriaNavigation = curso.CategoriaNavigation,
                    Modulos = curso.Modulos?.ToList() ?? new List<ModuloCurso>(),
                    Inscripciones = curso.Inscripciones?.ToList() ?? new List<Inscripcion>(),
                    CalificacionPromedio = promedio
                });
            }

            // Calcular estadísticas
            TotalEstudiantes = Cursos.Sum(c => c.Inscripciones?.Count ?? 0);
            TotalModulos = Cursos.Sum(c => c.Modulos?.Count ?? 0);

            var todasCalificaciones = Cursos
                .Where(c => c.CalificacionPromedio > 0)
                .Select(c => c.CalificacionPromedio)
                .ToList();

            PromedioCalificaciones = todasCalificaciones.Any() ? todasCalificaciones.Average() : 0;
        }
    }

    public class CursoInfo
    {
        public int idCursos { get; set; }
        public string titulo { get; set; } = string.Empty;
        public string descripcion { get; set; } = string.Empty;
        public decimal precio { get; set; }
        public int docenteId { get; set; }
        public int categoria_id { get; set; }
        public DateTime fecha_creacion { get; set; }
        public virtual Categoria? CategoriaNavigation { get; set; }
        public List<ModuloCurso> Modulos { get; set; } = new List<ModuloCurso>();
        public List<Inscripcion> Inscripciones { get; set; } = new List<Inscripcion>();
        public double CalificacionPromedio { get; set; }
    }
}