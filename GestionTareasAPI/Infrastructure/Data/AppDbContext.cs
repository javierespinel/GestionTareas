using GestionTareasAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GestionTareasAPI.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Usuario> Usuarios => Set<Usuario>();
        public DbSet<Tarea> Tareas => Set<Tarea>();

        protected override void OnModelCreating(ModelBuilder b)
        {
            base.OnModelCreating(b);


            b.Entity<Usuario>(e =>
            {
                e.ToTable("Usuarios");
                e.HasKey(x => x.Id);

                e.Property(x => x.Nombre)
                 .IsRequired()
                 .HasMaxLength(100);

                e.Property(x => x.Email)
                 .IsRequired()
                 .HasMaxLength(200);

                e.HasIndex(x => x.Email).IsUnique();
            });


            b.Entity<Tarea>(e =>
            {
                e.ToTable("Tareas");
                e.HasKey(x => x.Id);

                e.Property(x => x.Titulo)
                 .IsRequired()
                 .HasMaxLength(100);

                e.Property(x => x.Descripcion)
                 .HasMaxLength(1000);

                e.Property(x => x.FechaCreacion)
                 .HasDefaultValueSql("GETUTCDATE()")
                 .ValueGeneratedOnAdd();

                e.Property(x => x.FechaVencimiento)
                 .IsRequired();

                e.Property(x => x.Estado)
                 .HasConversion<int>()
                 .IsRequired();

                e.HasOne(x => x.Usuario)
                 .WithMany(u => u.Tareas)
                 .HasForeignKey(x => x.UsuarioId)
                 .OnDelete(DeleteBehavior.Cascade);

                e.ToTable(t => t.HasCheckConstraint(
                    "CK_Tareas_Fechas", "[FechaVencimiento] >= [FechaCreacion]"));
            });
        }
    }
}