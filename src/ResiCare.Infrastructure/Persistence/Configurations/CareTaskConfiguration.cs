using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ResiCare.Domain.Entities;

namespace ResiCare.Infrastructure.Persistence.Configurations;

public class CareTaskConfiguration : IEntityTypeConfiguration<CareTask>
{
    public void Configure(EntityTypeBuilder<CareTask> builder)
    {
        builder.ToTable("CareTasks");
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Label).HasMaxLength(200).IsRequired();
        builder.Property(t => t.Status).HasConversion<int>();

        builder.HasOne<Resident>()
            .WithMany()
            .HasForeignKey(t => t.ResidentId)
            .OnDelete(DeleteBehavior.Restrict);

        // CompletedByCaregiverId est Guid? -> EF en fait automatiquement une FK
        // optionnelle (la colonne accepte NULL tant que la tâche n'est pas faite).
        builder.HasOne<Caregiver>()
            .WithMany()
            .HasForeignKey(t => t.CompletedByCaregiverId)
            .OnDelete(DeleteBehavior.Restrict);

        // On interroge les tâches d'un résident pour une date donnée (Jalon 3).
        builder.HasIndex(t => new { t.ResidentId, t.ScheduledDate });
    }
}
