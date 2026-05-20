using TPI_2026.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TPI_2026.Infrastructure.Persistance.Configurations;

public class MedicalHistoryConfiguration : IEntityTypeConfiguration<MedicalHistory>
{
    public void Configure(EntityTypeBuilder<MedicalHistory> builder)
    {
        // Config de los atributos  de MedicalHistory
        builder.ToTable("MedicalHistories");
        builder.HasKey(medicalHistory => medicalHistory.Id);
        builder.Property(medicalHistory => medicalHistory.Diagnostic).HasMaxLength(2000).IsRequired();
        builder.Property(medicalHistory => medicalHistory.DateTime).IsRequired();
        builder.HasOne(medicalHistory => medicalHistory.Patient)
        
        // Relacion inversa de MedicalHistory con Patient
        .WithMany(patient => patient.MedicalHistories)
        .HasForeignKey(medicalHistory => medicalHistory.PatientId)
        .OnDelete(DeleteBehavior.Cascade);
    }
}   