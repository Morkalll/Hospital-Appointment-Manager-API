using TPI_2026.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class RoomConfiguration : IEntityTypeConfiguration<Room>
{
    public void Configure(EntityTypeBuilder<Room> builder)
    {
        builder.ToTable("Rooms");
        builder.HasKey(room => room.Id);
        builder.Property(room => room.Number).HasMaxLength(20).IsRequired();
        builder.Property(room => room.Specialty).HasConversion<string>();

        builder.HasMany(room => room.Appointments)
            .WithOne(appointment => appointment.Room)
            .HasForeignKey(appointment => appointment.RoomId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}