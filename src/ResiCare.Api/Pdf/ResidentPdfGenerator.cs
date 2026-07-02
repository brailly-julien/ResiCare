using QuestPDF.Fluent;
using QuestPDF.Helpers;
using ResiCare.Application.Caregivers;
using ResiCare.Application.Residents.Queries.GetResidentById;
using ResiCare.Domain.Enums;

namespace ResiCare.Api.Pdf;

/// <summary>
/// Génère la fiche résident en PDF (QuestPDF). La génération d'un document de sortie est
/// une préoccupation de présentation : on la garde donc dans la couche API.
/// </summary>
public static class ResidentPdfGenerator
{
    public static byte[] Generate(ResidentDashboardDto r, IReadOnlyList<CaregiverDto> caregivers)
    {
        string Caregiver(Guid id)
        {
            var c = caregivers.FirstOrDefault(x => x.Id == id);
            return c is null ? "—" : $"{c.LastName} {c.FirstName}";
        }

        return Document.Create(doc =>
        {
            doc.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(40);
                page.DefaultTextStyle(t => t.FontSize(11));

                page.Header().Text($"Fiche résident — {r.LastName} {r.FirstName}").FontSize(18).Bold();

                page.Content().PaddingVertical(12).Column(col =>
                {
                    col.Spacing(5);

                    col.Item().Text($"Chambre : {r.RoomNumber}");
                    col.Item().Text($"Naissance : {r.BirthDate:dd/MM/yyyy}  ·  Admission : {r.AdmissionDate:dd/MM/yyyy}");
                    col.Item().Text($"Référent : {Caregiver(r.ReferentCaregiverId)}");
                    if (r.IsArchived)
                        col.Item().Text("Résident archivé").Bold();

                    col.Item().PaddingTop(8).Text("Autonomie").Bold().FontSize(13);
                    col.Item().Text($"Boire et manger : {Autonomy(r.Dependency.Eating)}");
                    col.Item().Text($"Éliminer : {Autonomy(r.Dependency.Elimination)}");
                    col.Item().Text($"Se mouvoir : {Autonomy(r.Dependency.Mobility)}");
                    col.Item().Text($"Se vêtir : {Autonomy(r.Dependency.Dressing)}");
                    col.Item().Text($"Être propre : {Autonomy(r.Dependency.Hygiene)}");

                    col.Item().PaddingTop(8).Text("Risques").Bold().FontSize(13);
                    col.Item().Text($"Chute : {Risk(r.FallRisk)}  ·  Escarre : {Risk(r.PressureSoreRisk)}  ·  Dénutrition : {Risk(r.MalnutritionRisk)}");

                    if (HasInfo(r))
                    {
                        col.Item().PaddingTop(8).Text("Informations").Bold().FontSize(13);
                        if (!string.IsNullOrWhiteSpace(r.AttendingPhysician)) col.Item().Text($"Médecin traitant : {r.AttendingPhysician}");
                        if (!string.IsNullOrWhiteSpace(r.EmergencyContactName)) col.Item().Text($"Contact : {r.EmergencyContactName}");
                        if (!string.IsNullOrWhiteSpace(r.EmergencyContactPhone)) col.Item().Text($"Téléphone : {r.EmergencyContactPhone}");
                        if (!string.IsNullOrWhiteSpace(r.Occupation)) col.Item().Text($"Métier : {r.Occupation}");
                        if (!string.IsNullOrWhiteSpace(r.Interests)) col.Item().Text($"Intérêts : {r.Interests}");
                        if (!string.IsNullOrWhiteSpace(r.Family)) col.Item().Text($"Famille : {r.Family}");
                    }

                    col.Item().PaddingTop(8).Text("Observations récentes").Bold().FontSize(13);
                    if (r.RecentObservations.Count == 0)
                        col.Item().Text("Aucune.").Italic();
                    foreach (var o in r.RecentObservations)
                        col.Item().Text($"• [{ObsCategory(o.Category)}] {o.CreatedAt:dd/MM/yyyy HH:mm} — {o.Content}");

                    col.Item().PaddingTop(8).Text("Tâches du jour").Bold().FontSize(13);
                    if (r.TodayTasks.Count == 0)
                        col.Item().Text("Aucune.").Italic();
                    foreach (var t in r.TodayTasks)
                        col.Item().Text($"• {t.Label} — {(t.Status == CareTaskStatus.Done ? "Fait" : "À faire")}");
                });

                page.Footer().AlignCenter().Text($"ResiCare · généré le {DateTime.Now:dd/MM/yyyy HH:mm}").FontSize(9);
            });
        }).GeneratePdf();
    }

    private static bool HasInfo(ResidentDashboardDto r) =>
        !string.IsNullOrWhiteSpace(r.AttendingPhysician)
        || !string.IsNullOrWhiteSpace(r.EmergencyContactName)
        || !string.IsNullOrWhiteSpace(r.EmergencyContactPhone)
        || !string.IsNullOrWhiteSpace(r.Occupation)
        || !string.IsNullOrWhiteSpace(r.Interests)
        || !string.IsNullOrWhiteSpace(r.Family);

    private static string Autonomy(AutonomyLevel level) => level switch
    {
        AutonomyLevel.Independent => "Autonome",
        AutonomyLevel.PartialHelp => "Aide partielle",
        AutonomyLevel.Dependent => "Dépendant",
        _ => level.ToString()
    };

    private static string Risk(RiskLevel level) => level switch
    {
        RiskLevel.None => "Aucun",
        RiskLevel.Low => "Faible",
        RiskLevel.Moderate => "Modéré",
        RiskLevel.High => "Élevé",
        _ => level.ToString()
    };

    private static string ObsCategory(ObservationCategory category) => category switch
    {
        ObservationCategory.Care => "Soin",
        ObservationCategory.Behaviour => "Comportement",
        ObservationCategory.Nutrition => "Alimentation",
        ObservationCategory.Medical => "Médical",
        ObservationCategory.Other => "Autre",
        _ => category.ToString()
    };
}
