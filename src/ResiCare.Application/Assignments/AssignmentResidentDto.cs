using ResiCare.Domain.Enums;

namespace ResiCare.Application.Assignments;

/// <summary>Affectation côté soignant : le résident dont il a la charge — « mes résidents du jour ».</summary>
public record AssignmentResidentDto(
    Guid Id,
    Guid ResidentId, string ResidentFirstName, string ResidentLastName, string RoomNumber,
    DateOnly Date, Shift Shift);
