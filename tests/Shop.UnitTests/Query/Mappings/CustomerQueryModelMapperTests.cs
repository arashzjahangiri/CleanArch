using System;
using AwesomeAssertions;
using Shop.Domain.Entities.CustomerAggregate;
using Shop.Domain.Entities.CustomerAggregate.Events;
using Shop.Query.Mappings;
using Xunit;
using Xunit.Categories;

namespace Shop.UnitTests.Query.Mappings;

[UnitTest]
public class CustomerQueryModelMapperTests
{
    private static readonly Guid Id = Guid.NewGuid();
    private static readonly DateTime DateOfBirth = new(1990, 5, 17);

    public static TheoryData<CustomerBaseEvent> CustomerEvents => new()
    {
        new CustomerCreatedEvent(Id, "John", "Doe", EGender.Male, "john.doe@hostname.com", DateOfBirth),
        new CustomerUpdatedEvent(Id, "John", "Doe", EGender.Male, "john.doe@hostname.com", DateOfBirth),
        new CustomerDeletedEvent(Id, "John", "Doe", EGender.Male, "john.doe@hostname.com", DateOfBirth)
    };

    [Theory]
    [MemberData(nameof(CustomerEvents))]
    public void Should_MapEveryProperty_When_ProjectingEventToQueryModel(CustomerBaseEvent @event)
    {
        // Act
        var queryModel = @event.ToQueryModel();

        // Assert
        queryModel.Should().NotBeNull();
        queryModel.Id.Should().Be(Id);
        queryModel.FirstName.Should().Be("John");
        queryModel.LastName.Should().Be("Doe");
        queryModel.Gender.Should().Be(nameof(EGender.Male));
        queryModel.Email.Should().Be("john.doe@hostname.com");
        queryModel.DateOfBirth.Should().Be(DateOfBirth);
        queryModel.FullName.Should().Be("John Doe");
    }

    [Fact]
    public void Should_MapGenderAsItsName_When_GenderIsFemale()
    {
        // Arrange
        var @event = new CustomerCreatedEvent(Id, "Jane", "Roe", EGender.Female, "jane.roe@hostname.com", DateOfBirth);

        // Act
        var queryModel = @event.ToQueryModel();

        // Assert
        queryModel.Gender.Should().Be(nameof(EGender.Female));
        queryModel.Gender.Should().NotBe(((int)EGender.Female).ToString());
    }
}
