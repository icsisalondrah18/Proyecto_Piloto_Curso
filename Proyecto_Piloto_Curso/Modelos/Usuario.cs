using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proyecto_Piloto_Curso.Modelos
{
    public class Usuario
    {
        [Key]
        public int idUsuarios { get; set; }

        [Required(ErrorMessage = "El nombre de usuario es obligatorio")]
        public string? username { get; set; }

        [Required(ErrorMessage = "El nombre completo es obligatorio")]
        public string? nombre { get; set; }

        [Required(ErrorMessage = "El correo es obligatorio")]
        [EmailAddress(ErrorMessage = "Correo inválido")]
        public string? correo { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [DataType(DataType.Password)]
        public string? contrasena { get; set; }

        public string? rol { get; set; } = "Usuario";

        public DateTime fecha_creacion { get; set; } = DateTime.Now;

        public virtual ICollection<Tarjeta> Tarjetas { get; set; } = new List<Tarjeta>();

        public virtual ICollection<Inscripcion> Inscripciones { get; set; } = new List<Inscripcion>();

        public virtual ICollection<Resena> Resenas { get; set; } = new List<Resena>();
    }

    public class Categoria
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int idCat { get; set; }

        [Required]
        [StringLength(100)]
        public string nombre { get; set; } = string.Empty;

        public virtual ICollection<Curso> Cursos { get; set; } = new List<Curso>();
    }

    public class Docente
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int idDocente { get; set; }

        [StringLength(100)]
        public string? area { get; set; }

        public string? biografia { get; set; }

        [ForeignKey("idDocente")]
        public virtual Usuario? UsuarioNavigation { get; set; }

        public virtual ICollection<Curso> Cursos { get; set; } = new List<Curso>();
    }

    public class Curso
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int idCursos { get; set; }

        [Required]
        [StringLength(255)]
        public string titulo { get; set; } = string.Empty;

        public string? descripcion { get; set; }

        public decimal precio { get; set; }

        [Required]
        public int docenteId { get; set; }

        [Required]
        public int categoria_id { get; set; }

        public DateTime fecha_creacion { get; set; } = DateTime.Now;

        [ForeignKey("docenteId")]
        public virtual Docente? DocenteNavigation { get; set; }

        [ForeignKey("categoria_id")]
        public virtual Categoria? CategoriaNavigation { get; set; }

        public virtual ICollection<ModuloCurso> Modulos { get; set; } = new List<ModuloCurso>();

        public virtual ICollection<Objetivo> Objetivos { get; set; } = new List<Objetivo>();

        public virtual ICollection<Inscripcion> Inscripciones { get; set; } = new List<Inscripcion>();

        public virtual ICollection<Resena> Resenas { get; set; } = new List<Resena>(); //nueva
    }

    public class Objetivo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int idObjetivo { get; set; }

        [Required]
        public int curso_id { get; set; }

        public string? descripcion { get; set; }

        [ForeignKey("curso_id")]
        public virtual Curso? CursoNavigation { get; set; }
    }

    public class ModuloCurso
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int idModulo { get; set; }

        [Required]
        public int cursoId { get; set; }

        [StringLength(255)]
        public string? titulo { get; set; }

        [StringLength(20)]
        public string? tipo_contenido { get; set; }

        public string? url_video { get; set; }

        public int duracion { get; set; }

        public int orden { get; set; }

        [ForeignKey("cursoId")]
        public virtual Curso? CursoNavigation { get; set; }
    }

    public class Tarjeta
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int idTarjeta { get; set; }

        [Required]
        public int usuario_id { get; set; }

        [StringLength(20)]
        public string? numero_tarjeta { get; set; }

        [StringLength(100)]
        public string? nombre_titular { get; set; }

        [StringLength(7)]
        public string? fecha_expiracion { get; set; }

        [StringLength(20)]
        public string? tipo_tarjeta { get; set; }  // Visa, Mastercard

        [StringLength(10)]
        public string? cvv { get; set; }//CVV

        [ForeignKey("usuario_id")]
        public virtual Usuario? UsuarioNavigation { get; set; }
    }

    public class Inscripcion
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int idInscripciones { get; set; }

        [Required]
        public int usuario_id { get; set; }

        [Required]
        public int curso_id { get; set; }

        public decimal progreso { get; set; } = 0;

        public DateTime fecha_inscripcion { get; set; } = DateTime.Now;

        [ForeignKey("usuario_id")]
        public virtual Usuario? UsuarioNavigation { get; set; }

        [ForeignKey("curso_id")]
        public virtual Curso? CursoNavigation { get; set; }
    }

    public class Resena
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int idResenas { get; set; }

        [Required]
        public int usuario_id { get; set; }

        [Required]
        public int curso_id { get; set; }

        public int calificacion { get; set; }

        public string? comentario { get; set; }

        public DateTime fecha_creacion { get; set; } = DateTime.Now;

        [ForeignKey("usuario_id")]
        public virtual Usuario? UsuarioNavigation { get; set; }

        [ForeignKey("curso_id")]
        public virtual Curso? CursoNavigation { get; set; }
    }
    public class Descuento
    {
        [Key]
        public int idDescuentos { get; set; }
        public string? codigo { get; set; }
        public string? tipo { get; set; }
        public decimal? valor { get; set; }
        public int? max_usos { get; set; }
        public int? usos_actuales { get; set; }
        public DateTime? fecha_inicio { get; set; }
        public DateTime? fecha_fin { get; set; }
        public bool? activo { get; set; }
        public DateTime? fecha_creacion { get; set; }
    }
    public class Pago
    {
        [Key]
        public int idPagos { get; set; }
        public int? usuario_id { get; set; }
        public int? curso_id { get; set; }
        public int? tarjeta_id { get; set; }
        public decimal? precio_original { get; set; }
        public int? descuento_id { get; set; }
        public decimal? precio_final { get; set; }
        public DateTime? fecha_creacion { get; set; }
    }
}
