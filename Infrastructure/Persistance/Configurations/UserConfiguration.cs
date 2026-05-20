using TPI_2026.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TPI_2026.Infrastructure.Persistance.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        builder.HasKey(user => user.Id);
        builder.Property(user => user.Name).HasMaxLength(150).IsRequired();
        builder.Property(user => user.Email).HasMaxLength(200).IsRequired();
        builder.HasIndex(user => user.Email).IsUnique();
        builder.Property(user => user.Password).IsRequired();
        builder.Property(user => user.Role).HasConversion<string>();

        builder.HasDiscriminator<string>("UserType")
            .HasValue<Patient>("Patient")
            .HasValue<Doctor>("Doctor")
            .HasValue<Recepcionist>("Recepcionist")
            .HasValue<Administrator>("Administrator");
    }
}