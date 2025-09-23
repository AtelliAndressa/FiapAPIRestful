using Microsoft.EntityFrameworkCore;
using Infrastructure.Data;

namespace API
{
    public static class MigrationExtensions
    {
        public static void ApplyMigrations(this WebApplication app)
        {
            // Usamos um 'scope' para obter uma instância do DbContext
            using var scope = app.Services.CreateScope();
            
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            
            // Aplica qualquer migração pendente ao banco de dados
            dbContext.Database.Migrate();
        }
    }
}