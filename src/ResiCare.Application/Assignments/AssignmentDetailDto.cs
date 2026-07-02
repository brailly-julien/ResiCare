using ResiCare.Domain.Enums;

namespace ResiCare.Application.Assignments;

/// <summary>Affectation enrichie des noms (soignant + résident) — vue planning du responsable.</summary>
public record AssignmentDetailDto(
    Guid Id,
    Guid CaregiverId, string CaregiverFirstName, string CaregiverLastName,
    Guid ResidentId, string ResidentFirstName, string ResidentLastName, string RoomNumber,
    DateOnly Date, Shift Shift);
