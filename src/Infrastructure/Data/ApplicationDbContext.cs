using Core.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Aluno> Alunos { get; set; }

        public DbSet<Turma> Turmas { get; set; }

        public DbSet<Matricula> Matriculas { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Aluno>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("NEWSEQUENTIALID()");
                entity.HasIndex(e => e.Cpf).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();
            });

            builder.Entity<Turma>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("NEWSEQUENTIALID()");
            });

            builder.Entity<Matricula>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("NEWSEQUENTIALID()");
                entity.HasOne(m => m.Aluno).WithMany().HasForeignKey(m => m.AlunoId);
                entity.HasOne(m => m.Turma).WithMany(t => t.Matriculas).HasForeignKey(m => m.TurmaId);
            });
        }
    }
}
