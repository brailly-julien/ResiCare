using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ResiCare.Domain.Entities;

namespace ResiCare.Infrastructure.Persistence.Configurations;

public class CaregiverConfiguration : IEntityTypeConfiguration<Caregiver>
{
    public void Configure(EntityTypeBuilder<Caregiver> builder)
    {
        builder.ToTable("Caregivers");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.FirstName).HasMaxLength(100).IsRequired();
        builder.Property(c => c.LastName).HasMaxLength(100).IsRequired();
        builder.Property(c => c.Role).HasConversion<int>();

        // Email = identifiant de connexion : requis et UNIQUE (un compte par email).
        builder.Property(c => c.Email).HasMaxLength(256).IsRequired();
        builder.HasIndex(c => c.Email).IsUnique();

        // Empreinte PBKDF2 « itérations.sel.clé » : large marge de longueur.
        builder.Property(c => c.PasswordHash).HasMaxLength(512).IsRequired();
    }
}
