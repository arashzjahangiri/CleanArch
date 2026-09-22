using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Shop.Core.Extensions;
using Shop.Core.SharedKernel;
using Shop.Domain.Entities.CustomerAggregate.Events;
using Shop.Query.Abstractions;
using Shop.Query.Application.Customer.Queries;
using Shop.Query.Mappings;
using Shop.Query.QueriesModel;

namespace Shop.Query.EventHandlers;

public class CustomerEventHandler(
    ISynchronizeDb synchronizeDb,
    ICacheService cacheService,
    ILogger<CustomerEventHandler> logger) :
    INotificationHandler<CustomerCreatedEvent>,
    INotificationHandler<CustomerUpdatedEvent>,
    INotificationHandler<CustomerDeletedEvent>
{
    public async Task Handle(CustomerCreatedEvent notification, CancellationToken cancellationToken)
    {
        LogEvent(notification);

        var customerQueryModel = notification.ToQueryModel();
        await synchronizeDb.UpsertAsync(customerQueryModel, filter => filter.Id == customerQueryModel.Id, cancellationToken);
        await ClearCacheAsync(notification, cancellationToken);
    }

    public async Task Handle(CustomerDeletedEvent notification, CancellationToken cancellationToken)
    {
        LogEvent(notification);

        await synchronizeDb.DeleteAsync<CustomerQueryModel>(filter => filter.Email == notification.Email, cancellationToken);
        await ClearCacheAsync(notification, cancellationToken);
    }

    public async Task Handle(CustomerUpdatedEvent notification, CancellationToken cancellationToken)
    {
        LogEvent(notification);

        var customerQueryModel = notification.ToQueryModel();
        await synchronizeDb.UpsertAsync(customerQueryModel, filter => filter.Id == customerQueryModel.Id, cancellationToken);
        await ClearCacheAsync(notification, cancellationToken);
    }

    private async Task ClearCacheAsync(CustomerBaseEvent @event, CancellationToken cancellationToken)
    {
        var cacheKeys = new[] { nameof(GetAllCustomerQuery), $"{nameof(GetCustomerByIdQuery)}_{@event.Id}" };
        await cacheService.RemoveAsync(cacheKeys, cancellationToken);
    }

    private void LogEvent<TEvent>(TEvent @event) where TEvent : CustomerBaseEvent =>
        logger.LogInformation("----- Triggering the event {EventName}, model: {EventModel}", typeof(TEvent).Name, @event.ToJson());
}