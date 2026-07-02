using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ResiCare.Domain.Entities;

namespace ResiCare.Infrastructure.Persistence.Configurations;

public class PrescriptionConfiguration : IEntityTypeConfiguration<Prescription>
{
    public void Configure(EntityTypeBuilder<Prescription> builder)
    {
        builder.ToTable("Prescriptions");
        builder.HasKey(p => p.Id);

        builder.Property(p => p.MedicationName).HasMaxLength(200).IsRequired();
        builder.Property(p => p.Dosage).HasMaxLength(100).IsRequired();
        builder.Property(p => p.Posology).HasMaxLength(200).IsRequired();
        builder.Property(p => p.Instructions).HasMaxLength(1000);
        builder.Property(p => p.Route).HasConversion<int>();

        builder.HasOne<Resident>()
            .WithMany()
            .HasForeignKey(p => p.ResidentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(p => p.ResidentId);
    }
}
