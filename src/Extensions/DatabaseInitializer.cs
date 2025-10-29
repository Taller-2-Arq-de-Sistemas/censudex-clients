
using censudex_clients_service.src.Data;
using Microsoft.EntityFrameworkCore;

namespace censudex_clients_service.src.Extensions
{
    public static class DatabaseInitializer
    {
        public static void InitializeDatabase(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();
 
            db.Database.Migrate();
 
            if (!db.Clients.Any())
            {
                Seeder.Seed(db);
            }
        }

    }
}