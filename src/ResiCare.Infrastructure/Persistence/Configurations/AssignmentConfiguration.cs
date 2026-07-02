using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ResiCare.Domain.Entities;

namespace ResiCare.Infrastructure.Persistence.Configurations;

public class AssignmentConfiguration : IEntityTypeConfiguration<Assignment>
{
    public void Configure(EntityTypeBuilder<Assignment> builder)
    {
        builder.ToTable("Assignments");
        builder.HasKey(a => a.Id);

        builder.Property(a => a.Shift).HasConversion<int>();

        // Une affectation lie un soignant et un résident (FK par Id, pas de navigation).
        builder.HasOne<Caregiver>()
            .WithMany()
            .HasForeignKey(a => a.CaregiverId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Resident>()
            .WithMany()
            .HasForeignKey(a => a.ResidentId)
            .OnDelete(DeleteBehavior.Restrict);

        // Un seul soignant par résident et par poste sur une date donnée.
        builder.HasIndex(a => new { a.ResidentId, a.Date, a.Shift }).IsUnique();
    }
}
