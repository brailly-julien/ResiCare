using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ResiCare.Domain.Entities;

namespace ResiCare.Infrastructure.Persistence.Configurations;

public class MedicationAdministrationConfiguration : IEntityTypeConfiguration<MedicationAdministration>
{
    public void Configure(EntityTypeBuilder<MedicationAdministration> builder)
    {
        builder.ToTable("MedicationAdministrations");
        builder.HasKey(m => m.Id);

        builder.Property(m => m.Status).HasConversion<int>();
        builder.Property(m => m.Notes).HasMaxLength(1000);

        // Une administration appartient à une prescription et à un soignant (FK par Id).
        builder.HasOne<Prescription>()
            .WithMany()
            .HasForeignKey(m => m.PrescriptionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Caregiver>()
            .WithMany()
            .HasForeignKey(m => m.CaregiverId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(m => new { m.PrescriptionId, m.AdministeredAt });
    }
}
