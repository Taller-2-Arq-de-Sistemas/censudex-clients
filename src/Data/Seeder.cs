
using censudex_clients_service.src.Models;

namespace censudex_clients_service.src.Data
{
    
    /// <summary>
    /// Provides database seeding functionality for initial application data.
    /// </summary>
    /// <remarks>
    /// This static class contains methods to populate the database with initial
    /// test data and default users for development and demonstration purposes.
    /// </remarks>
    public static class Seeder
    {
        
        /// <summary>
        /// Seeds the database with initial client data including admin and test users.
        /// </summary>
        /// <param name="db">The database context to seed with data.</param>
        /// <remarks>
        /// <para>
        /// This method creates the following test users:
        /// </para>
        /// <list type="bullet">
        /// <item>
        /// <description>Admin user (Role 1) with credentials: admin@censudex.cl / Password1234!</description>
        /// </item>
        /// <item>
        /// <description>Regular users (Role 0) with test data and password: Password1234!</description>
        /// </item>
        /// </list>
        /// <para>
        /// All passwords are securely hashed using BCrypt before storage.
        /// </para>
        /// <example>
        /// The following example shows how to use the Seed method:
        /// <code>
        /// using (var context = new ApplicationDBContext(options))
        /// {
        ///     Seeder.Seed(context);
        /// }
        /// </code>
        /// </example>
        /// </remarks>
        /// <exception cref="ArgumentNullException">Thrown when the db parameter is null.</exception>
        /// <exception cref="DbUpdateException">Thrown when there is an error saving changes to the database.</exception>
        public static void Seed(ApplicationDBContext db)
        {
            var passwordAdminHash = BCrypt.Net.BCrypt.HashPassword("Password1234!");
            var passwordHash = BCrypt.Net.BCrypt.HashPassword("Password1234!");

            var clients = new List<Client>
            {
                new Client
                {
                    Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    FirstName = "Admin",
                    LastNames = "Censudex",
                    Email = "admin@censudex.cl",
                    Username = "admin",
                    Birthdate = new DateOnly(1990, 1, 1),
                    Address = "Headquarters",
                    PhoneNumber = "+56900000000",
                    PasswordHash = passwordAdminHash,
                    Role = 1,
                    IsActive = true,
                    CreatedAt = DateOnly.FromDateTime(DateTime.UtcNow)
                },
                new Client
                {
                    Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                    FirstName = "John",
                    LastNames = "Doe",
                    Email = "john@censudex.cl",
                    Username = "john",
                    Birthdate = new DateOnly(1995, 5, 10),
                    Address = "Example Street 123",
                    PhoneNumber = "+56911111111",
                    PasswordHash = passwordHash,
                    Role = 0,
                    IsActive = true,
                    CreatedAt = DateOnly.FromDateTime(DateTime.UtcNow)
                },
                new Client
                {
                    Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                    FirstName = "Jane",
                    LastNames = "Smith",
                    Email = "jane@censudex.cl",
                    Username = "jane",
                    Birthdate = new DateOnly(2000, 3, 15),
                    Address = "Another Street 456",
                    PhoneNumber = "+56922222222",
                    PasswordHash = passwordHash,
                    Role = 0,
                    IsActive = true,
                    CreatedAt = DateOnly.FromDateTime(DateTime.UtcNow)
                },
                new Client
                {
                    Id = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                    FirstName = "Mark",
                    LastNames = "Brown",
                    Email = "mark@censudex.cl",
                    Username = "mark",
                    Birthdate = new DateOnly(1998, 10, 20),
                    Address = "Third Street 789",
                    PhoneNumber = "+56933333333",
                    PasswordHash = passwordHash,
                    Role = 0,
                    IsActive = true,
                    CreatedAt = DateOnly.FromDateTime(DateTime.UtcNow)
                }
            };
            db.Clients.AddRange(clients);
            db.SaveChanges();
        }
    }
}
