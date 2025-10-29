
using censudex_clients_service.src.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace censudex_clients_service.src.Data.Configurations
{
    public class ClientConfiguration : IEntityTypeConfiguration<Client>
    {
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