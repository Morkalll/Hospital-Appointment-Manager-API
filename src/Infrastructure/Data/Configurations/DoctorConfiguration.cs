using TPI_2026.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class DoctorConfiguration : IEntityTypeConfiguration<Doctor>
{
    public void Configure(EntityTypeBuilder<Doctor> builder)
    {
        builder.Property(doctor => doctor.Credential).HasMaxLength(50).IsRequired();
        builder.HasIndex(doctor => doctor.Credential).IsUnique();
        builder.Property(doctor => doctor.Specialty).HasConversion<string>();

        builder.HasMany(doctor => doctor.Rooms)
            .WithOne(room => room.Doctor)
            .HasForeignKey(room => room.DoctorId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(doctor => doctor.Appointments)
            .WithOne(appointment => appointment.Doctor)
            .HasForeignKey(appointment => appointment.DoctorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}