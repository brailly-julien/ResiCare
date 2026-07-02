using FluentAssertions;
using ResiCare.Application.Residents.Queries.GetResidents;
using ResiCare.Application.Tests.Common;
using ResiCare.Domain.Entities;
using ResiCare.Domain.Enums;

namespace ResiCare.Application.Tests.Residents;

public class GetResidentsQueryHandlerTests : DatabaseTestBase
{
    private async Task SeedThreeResidentsAsync()
    {
        var caregiver = new Caregiver("Paul", "Durand", CaregiverRole.Caregiver, "paul.durand@resicare.local", "hash");
        await using var seed = CreateContext();
        seed.Caregivers.Add(caregiver);
        seed.Residents.AddRange(
            new Resident("Jeanne", "Lefèvre", new DateOnly(1940, 5, 12), new DateOnly(2022, 9, 1), "101", TestData.AnyDependency(), caregiver.Id),
            new Resident("Robert", "Bernard", new DateOnly(1942, 7, 25), new DateOnly(2023, 1, 15), "102", TestData.AnyDependency(), caregiver.Id),
            new Resident("Germaine", "Petit", new DateOnly(1935, 11, 3), new DateOnly(2021, 5, 20), "103", TestData.AnyDependency(), caregiver.Id));
        await seed.SaveChangesAsync();
    }

    [Fact]
    public async Task Handle_WithoutSearch_ReturnsAllOrderedByLastName()
    {
        await SeedThreeResidentsAsync();
        await using var context = CreateContext();
        var handler = new GetResidentsQueryHandler(context);

        var result = await handler.Handle(new GetResidentsQuery(null), CancellationToken.None);

        result.Should().HaveCount(3);
        result.Select(r => r.LastName).Should().ContainInOrder("Bernard", "Lefèvre", "Petit");
    }

    [Fact]
    public async Task Handle_WithSearch_FiltersByName()
    {
        await SeedThreeResidentsAsync();
        await using var context = CreateContext();
        var handler = new GetResidentsQueryHandler(context);

        var result = await handler.Handle(new GetResidentsQuery("Bern"), CancellationToken.None);

        result.Should().ContainSingle().Which.LastName.Should().Be("Bernard");
    }
}
