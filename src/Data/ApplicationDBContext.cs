using censudex_clients_service.src.Data.Configurations;
using censudex_clients_service.src.Models;
using Microsoft.EntityFrameworkCore;
namespace censudex_clients_service.src.Data
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options)
            : base(options)
        {
        }

        public DbSet<Client> Clients { get; set; } = null!;
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new ClientConfiguration());
            base.OnModelCreating(modelBuilder); 
        }
    }

}