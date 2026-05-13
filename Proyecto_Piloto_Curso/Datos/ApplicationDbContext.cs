using Microsoft.EntityFrameworkCore;
using Proyecto_Piloto_Curso.Modelos;

namespace Proyecto_Piloto_Curso.Datos
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Docente> Docentes { get; set; }
        public DbSet<Curso> Cursos { get; set; }
        public DbSet<Objetivo> Objetivos { get; set; }
        public DbSet<ModuloCurso> ModulosCursos { get; set; }
        public DbSet<Tarjeta> Tarjetas { get; set; }
        public DbSet<Inscripcion> Inscripciones { get; set; }
        public DbSet<Resena> Resenas { get; set; }
        public DbSet<Descuento> Descuentos { get; set; }
        public DbSet<Pago> Pagos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configurar nombres de tablas
            modelBuilder.Entity<Usuario>().ToTable("usuarios");
            modelBuilder.Entity<Categoria>().ToTable("categorias");
            modelBuilder.Entity<Docente>().ToTable("docentes");
            modelBuilder.Entity<Curso>().ToTable("cursos");
            modelBuilder.Entity<Objetivo>().ToTable("objetivos");
            modelBuilder.Entity<ModuloCurso>().ToTable("modulosCurso");
            modelBuilder.Entity<Inscripcion>().ToTable("inscripciones");
            modelBuilder.Entity<Resena>().ToTable("resenas");
            modelBuilder.Entity<Descuento>().ToTable("descuentos");
            modelBuilder.Entity<Pago>().ToTable("pagos");
        }
    }
}