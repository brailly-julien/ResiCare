using FluentAssertions;
using ResiCare.Application.Assignments.Queries.GetMyAssignments;
using ResiCare.Application.Tests.Common;
using ResiCare.Domain.Entities;
using ResiCare.Domain.Enums;

namespace ResiCare.Application.Tests.Assignments;

public class GetMyAssignmentsQueryHandlerTests : DatabaseTestBase
{
    private static readonly DateOnly Today = new(2026, 6, 22);

    [Fact]
    public async Task Handle_ReturnsOnlyMyAssignmentsForTheDate()
    {
        var paul = new Caregiver("Paul", "Durand", CaregiverRole.Caregiver, "paul.durand@resicare.local", "h");
        var sophie = new Caregiver("Sophie", "Martin", CaregiverRole.Caregiver, "sophie.martin@resicare.local", "h");
        var jeanne = new Resident("Jeanne", "Lefèvre", new DateOnly(1940, 5, 12),
            new DateOnly(2022, 9, 1), "101", TestData.AnyDependency(), paul.Id);
        var robert = new Resident("Robert", "Bernard", new DateOnly(1942, 7, 25),
            new DateOnly(2023, 1, 15), "102", TestData.AnyDependency(), sophie.Id);

        await using (var seed = CreateContext())
        {
            seed.Caregivers.AddRange(paul, sophie);
            seed.Residents.AddRange(jeanne, robert);
            seed.Assignments.AddRange(
                new Assignment(paul.Id, jeanne.Id, Today, Shift.Morning),
                new Assignment(sophie.Id, robert.Id, Today, Shift.Morning)); // celle-ci ne doit PAS remonter
            await seed.SaveChangesAsync();
        }

        await using var context = CreateContext();
        var handler = new GetMyAssignmentsQueryHandler(context, new FakeCurrentUser(paul.Id));
        var result = await handler.Handle(new GetMyAssignmentsQuery(Today), CancellationToken.None);

        result.Should().ContainSingle();
        result[0].ResidentId.Should().Be(jeanne.Id);
        result[0].RoomNumber.Should().Be("101");
    }
}
