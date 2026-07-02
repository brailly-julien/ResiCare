using Microsoft.EntityFrameworkCore;
using ResiCare.Application.Common.Persistence;
using ResiCare.Domain.Entities;

namespace ResiCare.Infrastructure.Persistence;

/// <summary>
/// Le DbContext : la "session" EF Core. C'est lui qui fait le pont entre nos
/// objets C# et les tables SQL. Chaque DbSet correspond (par convention) à une table.
/// Il vit dans Infrastructure car c'est un détail technique : le Domaine l'ignore.
/// </summary>
public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    // EF injecte les options (provider, chaîne de connexion) via le constructeur :
    // c'est ce qui permet de configurer SQL Server depuis l'extérieur (l'API).
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // Set<T>() en propriété calculée : équivalent moderne de "{ get; set; }",
    // mais sans l'avertissement nullable (la propriété n'est jamais null).
    public DbSet<Resident> Residents => Set<Resident>();
    public DbSet<Caregiver> Caregivers => Set<Caregiver>();
    public DbSet<Observation> Observations => Set<Observation>();
    public DbSet<CareTask> CareTasks => Set<CareTask>();
    public DbSet<TimeEntry> TimeEntries => Set<TimeEntry>();
    public DbSet<Assignment> Assignments => Set<Assignment>();
    public DbSet<Prescription> Prescriptions => Set<Prescription>();
    public DbSet<MedicationAdministration> MedicationAdministrations => Set<MedicationAdministration>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Scanne CET assembly et applique automatiquement toutes les classes
        // IEntityTypeConfiguration<T>. Pas besoin de les enregistrer une par une.
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
