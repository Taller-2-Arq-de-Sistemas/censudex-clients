
using censudex_clients_service.src.Data;
using Microsoft.EntityFrameworkCore;

namespace censudex_clients_service.src.Extensions
{
    /// <summary>
    /// Provides extension methods for initializing and seeding the application database.
    /// </summary>
    /// <remarks>
    /// This static class contains methods to ensure the database is properly migrated
    /// and populated with initial data during application startup.
    /// </remarks>
    public static class DatabaseInitializer
    {
        /// <summary>
        /// Initializes the database by applying pending migrations and seeding initial data if empty.
        /// </summary>
        /// <param name="app">The application builder instance.</param>
        /// <remarks>
        /// <para>
        /// This extension method performs the following operations:
        /// </para>
        /// <list type="number">
        /// <item>
        /// <description>Applies any pending Entity Framework migrations to the database</description>
        /// </item>
        /// <item>
        /// <description>Checks if the database contains any existing client records</description>
        /// </item>
        /// <item>
        /// <description>Seeds the database with initial test data if no clients exist</description>
        /// </item>
        /// </list>
        /// <para>
        /// This method should be called during application startup, typically in the Program.cs file,
        /// after building the application but before running it.
        /// </para>
        /// <example>
        /// The following example shows how to use this method in Program.cs:
        /// <code>
        /// var app = builder.Build();
        /// 
        /// // Initialize database
        /// app.InitializeDatabase();
        /// 
        /// app.Run();
        /// </code>
        /// </example>
        /// <note type="important">
        /// This method uses a service scope to resolve the database context and should only be called
        /// after the application services have been fully configured.
        /// </note>
        /// </remarks>
        /// <exception cref="ArgumentNullException">Thrown when the app parameter is null.</exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the ApplicationDBContext service is not registered in the dependency injection container.
        /// </exception>
        /// <exception cref="DbUpdateException">
        /// Thrown when there is an error applying migrations to the database.
        /// </exception>
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