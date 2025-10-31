using censudex_clients_service.src.Data.Configurations;
using censudex_clients_service.src.Models;
using Microsoft.EntityFrameworkCore;
namespace censudex_clients_service.src.Data
{

    /// <summary>
    /// Represents the database context for the Censudex Clients Service application.
    /// </summary>
    /// <remarks>
    /// This class is responsible for managing database connections, entity sets,
    /// and configuring the entity framework model for the application.
    /// </remarks>
    public class ApplicationDBContext : DbContext
    {
        
        /// <summary>
        /// Initializes a new instance of the ApplicationDBContext class.
        /// </summary>
        /// <param name="options">The options to be used by the DbContext.</param>
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// Gets or sets the Clients entity set representing the clients table in the database.
        /// </summary>
        /// <value>
        /// A DbSet of Client entities that can be used to query and save instances of Client.
        /// </value>
        public DbSet<Client> Clients { get; set; } = null!;

        /// <summary>
        /// Configures the model that was discovered by convention from the entity types
        /// exposed in DbSet properties on your derived context.
        /// </summary>
        /// <param name="modelBuilder">
        /// The builder being used to construct the model for this context.
        /// </param>
        /// <remarks>
        /// This method applies entity configurations and sets up the database model
        /// including relationships, constraints, and entity configurations.
        /// </remarks>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new ClientConfiguration());
            base.OnModelCreating(modelBuilder);
        }
    }

}