using TPI_2026.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TPI_2026.Infrastructure.Data.Configurations;

public class PatientConfiguration : IEntityTypeConfiguration<Patient>
{
    public void Configure(EntityTypeBuilder<Patient> builder)
    {
        // Config de los atributos full picado de Patient
        builder.Property(patient => patient.Dni).HasMaxLength(8).IsRequired();
        builder.HasIndex(patient => patient.Dni).IsUnique();
        builder.Property(patient => patient.PhoneNumber).HasMaxLength(30);

        // Relación con Appointments (Turnos)
        builder.HasMany(patient => patient.Appointments)
            .WithOne(appointment => appointment.Patient)
            .HasForeignKey(appointment => appointment.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relación con MedicalHistories 
        builder.HasMany(patient => patient.MedicalHistories)
            .WithOne(medicalHistory => medicalHistory.Patient)
            .HasForeignKey(medicalHistory => medicalHistory.PatientId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
