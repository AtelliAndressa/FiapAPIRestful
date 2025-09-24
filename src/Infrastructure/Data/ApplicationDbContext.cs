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

                entity.Property(e => e.Nome).HasMaxLength(100);
                entity.Property(e => e.Email).HasMaxLength(50);
                entity.Property(e => e.Cpf).HasMaxLength(11).IsFixedLength();
            });

            builder.Entity<Turma>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("NEWSEQUENTIALID()");

                entity.Property(e => e.Nome).HasMaxLength(100);
                entity.Property(e => e.Descricao).HasMaxLength(300);
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
