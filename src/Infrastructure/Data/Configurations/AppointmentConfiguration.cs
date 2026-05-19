using TPI_2026.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
{
    public void Configure(EntityTypeBuilder<Appointment> builder)
    {
        builder.ToTable("Appointments");
        builder.HasKey(appointment => appointment.Id);
        builder.Property(appointment => appointment.DateTime).IsRequired();
        builder.Property(appointment => appointment.State).HasConversion<string>();

        builder.HasOne(appointment => appointment.MedicalHistory)
            .WithOne(medicalHistory => medicalHistory.Appointment)
            .HasForeignKey<MedicalHistory>(medicalHistory => medicalHistory.AppointmentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}