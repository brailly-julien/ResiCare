using ResiCare.Application.Common.Messaging;

namespace ResiCare.Application.Residents.Commands.ArchiveResident;

/// <summary>Archive un résident (soft delete). Ne renvoie rien (Unit).</summary>
public record ArchiveResidentCommand(Guid Id) : ICommand<Unit>;
