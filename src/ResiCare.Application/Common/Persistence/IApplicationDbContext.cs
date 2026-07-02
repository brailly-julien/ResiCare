using Microsoft.EntityFrameworkCore;
using ResiCare.Domain.Entities;

namespace ResiCare.Application.Common.Persistence;

/// <summary>
/// Abstraction du DbContext, vue par la couche Application. On l'introduit MAINTENANT,
/// car c'est le premier handler qui en a besoin (YAGNI : pas d'abstraction sans usage).
/// L'Application dépend ainsi des "abstractions" d'EF Core (DbSet) mais PAS du provider
/// SQL Server : ce dernier est le détail remplaçable, et il reste dans Infrastructure.
/// </summary>
public interface IApplicationDbContext
{
    DbSet<Resident> Residents { get; }
    DbSet<Caregiver> Caregivers { get; }
    DbSet<Observation> Observations { get; }
    DbSet<CareTask> CareTasks { get; }
    DbSet<TimeEntry> TimeEntries { get; }
    DbSet<Assignment> Assignments { get; }
    DbSet<Prescription> Prescriptions { get; }
    DbSet<MedicationAdministration> MedicationAdministrations { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
