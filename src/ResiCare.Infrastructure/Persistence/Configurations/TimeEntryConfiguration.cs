using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ResiCare.Domain.Entities;

namespace ResiCare.Infrastructure.Persistence.Configurations;

public class TimeEntryConfiguration : IEntityTypeConfiguration<TimeEntry>
{
    public void Configure(EntityTypeBuilder<TimeEntry> builder)
    {
        builder.ToTable("TimeEntries");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.ClockInAt).IsRequired();
        // ClockOutAt est DateTime? -> colonne nullable (pointage en cours tant qu'elle est NULL).

        // Un pointage appartient à un soignant (FK par Id, pas de navigation).
        builder.HasOne<Caregiver>()
            .WithMany()
            .HasForeignKey(e => e.CaregiverId)
            .OnDelete(DeleteBehavior.Restrict);

        // On interroge les pointages d'un soignant par date, et la présence du jour.
        builder.HasIndex(e => new { e.CaregiverId, e.ClockInAt });

        // Index UNIQUE FILTRÉ : au plus un pointage OUVERT (ClockOutAt NULL) par soignant.
        // C'est la base qui garantit la règle, pas seulement le check-then-act du handler
        // (évite une course). Le filtre sans crochets/guillemets est valable SQL Server ET SQLite.
        builder.HasIndex(e => e.CaregiverId)
            .IsUnique()
            .HasFilter("ClockOutAt IS NULL")
            .HasDatabaseName("UX_TimeEntries_CaregiverId_OpenShift");
    }
}
