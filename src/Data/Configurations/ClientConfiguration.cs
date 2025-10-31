
using censudex_clients_service.src.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace censudex_clients_service.src.Data.Configurations
{
    /// <summary>
    /// Configures the entity framework mappings for the <see cref="Client"/> entity.
    /// </summary>
    public class ClientConfiguration : IEntityTypeConfiguration<Client>
    {
        /// <summary>
        /// Configures the database schema for the <see cref="Client"/> entity.
        /// </summary>
        /// <param name="builder">The builder used to configure the entity type.</param>
        public void Configure(EntityTypeBuilder<Client> builder)
        {
            builder.ToTable("clients");

            // UUID default generator (PostgreSQL)
            builder.Property(c => c.Id)
                   .HasDefaultValueSql("gen_random_uuid()");

            builder.HasIndex(c => c.Email).IsUnique();
            builder.HasIndex(c => c.Username).IsUnique();

            builder.Property(c => c.IsActive)
                   .HasDefaultValue(true);

            builder.Property(c => c.CreatedAt)
                   .HasDefaultValueSql("CURRENT_TIMESTAMP");
        }
    }
}