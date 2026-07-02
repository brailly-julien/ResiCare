using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ResiCare.Domain.Entities;

namespace ResiCare.Infrastructure.Persistence.Configurations;

public class ResidentConfiguration : IEntityTypeConfiguration<Resident>
{
    public void Configure(EntityTypeBuilder<Resident> builder)
    {
        builder.ToTable("Residents");
        builder.HasKey(r => r.Id);

        builder.Property(r => r.FirstName).HasMaxLength(100).IsRequired();
        builder.Property(r => r.LastName).HasMaxLength(100).IsRequired();
        builder.Property(r => r.RoomNumber).HasMaxLength(20).IsRequired();

        // Profil de dépendance = objet-valeur "owned" -> stocké dans des colonnes de la
        // table Residents (Dependency_Eating, Dependency_Elimination, ...).
        // Profil de dépendance = objet-valeur "owned" -> colonnes Dependency_Eating, etc.
        builder.OwnsOne(r => r.Dependency, dependency =>
        {
            dependency.Property(d => d.Eating).HasConversion<int>();
            dependency.Property(d => d.Elimination).HasConversion<int>();
            dependency.Property(d => d.Mobility).HasConversion<int>();
            dependency.Property(d => d.Dressing).HasConversion<int>();
            dependency.Property(d => d.Hygiene).HasConversion<int>();
        });
        builder.Navigation(r => r.Dependency).IsRequired();

        // Risques (niveau stocké en int).
        builder.Property(r => r.FallRisk).HasConversion<int>();
        builder.Property(r => r.PressureSoreRisk).HasConversion<int>();
        builder.Property(r => r.MalnutritionRisk).HasConversion<int>();

        // Infos administratives + bio (optionnelles).
        builder.Property(r => r.AttendingPhysician).HasMaxLength(200);
        builder.Property(r => r.EmergencyContactName).HasMaxLength(200);
        builder.Property(r => r.EmergencyContactPhone).HasMaxLength(30);
        builder.Property(r => r.Occupation).HasMaxLength(200);
        builder.Property(r => r.Interests).HasMaxLength(1000);
        builder.Property(r => r.Family).HasMaxLength(1000);

        builder.Property(r => r.IsArchived).HasDefaultValue(false);

        // FK vers le soignant référent (sans propriété de navigation).
        builder.HasOne<Caregiver>()
            .WithMany()
            .HasForeignKey(r => r.ReferentCaregiverId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(r => new { r.LastName, r.FirstName });
    }
}
