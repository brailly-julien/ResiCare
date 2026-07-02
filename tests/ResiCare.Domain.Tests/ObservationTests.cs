using FluentAssertions;
using ResiCare.Domain.Entities;
using ResiCare.Domain.Enums;
using ResiCare.Domain.Exceptions;

namespace ResiCare.Domain.Tests;

public class ObservationTests
{
    private static Resident CreateResident() =>
        new("Jeanne", "Lefèvre", new DateOnly(1940, 5, 12), new DateOnly(2022, 9, 1),
            "101", TestData.AnyDependency(), Guid.NewGuid());

    [Fact]
    public void Create_OnActiveResident_Succeeds()
    {
        var resident = CreateResident();
        var caregiverId = Guid.NewGuid();

        var observation = Observation.Create(resident, caregiverId, ObservationCategory.Care, "RAS");

        observation.ResidentId.Should().Be(resident.Id);
        observation.CaregiverId.Should().Be(caregiverId);
        observation.Category.Should().Be(ObservationCategory.Care);
        observation.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void Create_OnArchivedResident_ThrowsResidentArchivedException()
    {
        var resident = CreateResident();
        resident.Archive();

        Action act = () => Observation.Create(resident, Guid.NewGuid(), ObservationCategory.Care, "RAS");

        act.Should().Throw<ResidentArchivedException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithBlankContent_ThrowsDomainException(string content)
    {
        var resident = CreateResident();

        Action act = () => Observation.Create(resident, Guid.NewGuid(), ObservationCategory.Care, content);

        act.Should().Throw<DomainException>();
    }
}
