using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ResiCare.Domain.Entities;

namespace ResiCare.Infrastructure.Persistence.Configurations;

public class ObservationConfiguration : IEntityTypeConfiguration<Observation>
{
    public void Configure(EntityTypeBuilder<Observation> builder)
    {
        builder.ToTable("Observations");
        builder.HasKey(o => o.Id);

        builder.Property(o => o.Content).HasMaxLength(2000).IsRequired();
        builder.Property(o => o.Category).HasConversion<int>();

        // Une observation appartient à un résident et à un soignant (FK par Id).
        builder.HasOne<Resident>()
            .WithMany()
            .HasForeignKey(o => o.ResidentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Caregiver>()
            .WithMany()
            .HasForeignKey(o => o.CaregiverId)
            .OnDelete(DeleteBehavior.Restrict);

        // On liste presque toujours les observations d'un résident, triées par date.
        builder.HasIndex(o => new { o.ResidentId, o.CreatedAt });
    }
}
