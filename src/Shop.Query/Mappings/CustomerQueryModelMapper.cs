using Shop.Domain.Entities.CustomerAggregate.Events;
using Shop.Query.QueriesModel;

namespace Shop.Query.Mappings;

public static class CustomerQueryModelMapper
{
    /// <summary>
    /// Projects a customer domain event onto its read-side query model.
    /// </summary>
    /// <param name="event">The domain event carrying the current customer state.</param>
    /// <returns>The query model written to the read database.</returns>
    public static CustomerQueryModel ToQueryModel(this CustomerBaseEvent @event) =>
        new(
            @event.Id,
            @event.FirstName,
            @event.LastName,
            @event.Gender.ToString(),
            @event.Email,
            @event.DateOfBirth);
}
