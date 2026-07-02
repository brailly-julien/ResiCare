using Microsoft.EntityFrameworkCore;
using ResiCare.Application.Common.Security;
using ResiCare.Domain.Entities;
using ResiCare.Domain.Enums;
using ResiCare.Domain.ValueObjects;

namespace ResiCare.Infrastructure.Persistence;

/// <summary>
/// Données de démonstration. Idempotent : ne fait rien si la base contient déjà des
/// résidents. On passe par les CONSTRUCTEURS du Domaine (données forcément valides).
/// Les mots de passe sont hachés via <see cref="IPasswordHasher"/> avant stockage.
/// Comptes de démo : marie.curie@resicare.local (Manager / "Manager123!"),
/// paul.durand@resicare.local et sophie.martin@resicare.local (Soignant / "Soignant123!").
/// </summary>
public static class ApplicationDbContextSeeder
{
    public static async Task SeedAsync(
        ApplicationDbContext context, IPasswordHasher passwordHasher, CancellationToken cancellationToken = default)
    {
        if (await context.Residents.AnyAsync(cancellationToken))
            return;

        var manager = new Caregiver("Marie", "Curie", CaregiverRole.Manager,
            "marie.curie@resicare.local", passwordHasher.Hash("Manager123!"));
        var paul = new Caregiver("Paul", "Durand", CaregiverRole.Caregiver,
            "paul.durand@resicare.local", passwordHasher.Hash("Soignant123!"));
        var sophie = new Caregiver("Sophie", "Martin", CaregiverRole.Caregiver,
            "sophie.martin@resicare.local", passwordHasher.Hash("Soignant123!"));
        context.Caregivers.AddRange(manager, paul, sophie);

        var jeanne = new Resident("Jeanne", "Lefèvre", new DateOnly(1938, 3, 12), new DateOnly(2022, 9, 1), "101",
            new DependencyProfile(AutonomyLevel.PartialHelp, AutonomyLevel.Dependent, AutonomyLevel.Dependent, AutonomyLevel.PartialHelp, AutonomyLevel.Dependent),
            paul.Id);
        jeanne.SetRisks(RiskLevel.High, RiskLevel.Moderate, RiskLevel.None);
        jeanne.SetPersonalInfo("Dr Martin", "Sophie Lefèvre (fille)", "06 12 34 56 78", "Institutrice", "Lecture, jardinage", "2 enfants, 4 petits-enfants");

        var robert = new Resident("Robert", "Bernard", new DateOnly(1942, 7, 25), new DateOnly(2023, 1, 15), "102",
            new DependencyProfile(AutonomyLevel.Independent, AutonomyLevel.PartialHelp, AutonomyLevel.PartialHelp, AutonomyLevel.Independent, AutonomyLevel.PartialHelp),
            sophie.Id);
        robert.SetRisks(RiskLevel.Moderate, RiskLevel.None, RiskLevel.None);
        robert.SetPersonalInfo("Dr Dubois", "Luc Bernard (fils)", "06 98 76 54 32", "Cheminot", "Belote, pétanque", "Veuf, 1 fils");

        var germaine = new Resident("Germaine", "Petit", new DateOnly(1935, 11, 3), new DateOnly(2021, 5, 20), "103",
            new DependencyProfile(AutonomyLevel.Dependent, AutonomyLevel.Dependent, AutonomyLevel.Dependent, AutonomyLevel.Dependent, AutonomyLevel.Dependent),
            paul.Id);
        germaine.SetRisks(RiskLevel.None, RiskLevel.High, RiskLevel.High);
        germaine.SetPersonalInfo("Dr Martin", "Paul Petit (neveu)", "07 11 22 33 44", "Couturière", "Tricot, musique", "Sans enfant");

        context.Residents.AddRange(jeanne, robert, germaine);

        context.Observations.AddRange(
            Observation.Create(jeanne, paul.Id, ObservationCategory.Nutrition, "A bien déjeuné ce midi."),
            Observation.Create(jeanne, sophie.Id, ObservationCategory.Behaviour, "Calme et souriante toute la journée."),
            Observation.Create(robert, sophie.Id, ObservationCategory.Medical, "Tension artérielle à surveiller."));

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var medsTask = new CareTask(jeanne.Id, "Distribution des médicaments", today);
        medsTask.Complete(paul.Id);

        context.CareTasks.AddRange(
            new CareTask(jeanne.Id, "Toilette du matin", today),
            medsTask,
            new CareTask(robert.Id, "Séance de kinésithérapie", today));

        // Affectations du jour (qui s'occupe de quelle chambre) — poste du matin.
        context.Assignments.AddRange(
            new Assignment(paul.Id, jeanne.Id, today, Shift.Morning),
            new Assignment(paul.Id, germaine.Id, today, Shift.Morning),
            new Assignment(sophie.Id, robert.Id, today, Shift.Morning));

        // Prescriptions + une administration de démonstration.
        var doliprane = new Prescription(jeanne.Id, "Paracétamol", "1000 mg", "3x/jour (matin, midi, soir)",
            MedicationRoute.Oral, today, null, "Si douleur ou fièvre.");
        var insuline = new Prescription(robert.Id, "Insuline lente", "12 UI", "1x/jour (matin)",
            MedicationRoute.Injectable, today, null, null);
        context.Prescriptions.AddRange(doliprane, insuline);

        context.MedicationAdministrations.Add(
            MedicationAdministration.Record(doliprane, paul.Id, AdministrationStatus.Given, "Prise du matin."));

        await context.SaveChangesAsync(cancellationToken);
    }
}
