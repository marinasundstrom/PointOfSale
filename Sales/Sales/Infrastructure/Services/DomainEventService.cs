using System;
using System.Reflection;
using System.Threading.Tasks;

using MassTransit;

using Sales.Application.Common.Interfaces;
using Sales.Contracts;
using Sales.Domain.Common;

namespace Sales.Infrastructure.Services
{
    public class DomainEventService : IDomainEventService
    {
        private readonly ILogger<DomainEventService> _logger;
        private readonly IBus _bus;
        private static readonly MethodInfo _busPublishMethod;

        static DomainEventService()
        {
            _busPublishMethod = typeof(IPublishEndpoint)
                .GetMethods()!
                .First(m => m.Name == "Publish");
        }

        public DomainEventService(ILogger<DomainEventService> logger, IBus bus)
        {
            _logger = logger;
            _bus = bus;
        }

        public async Task Publish(IDomainEvent domainEvent)
        {
            _logger.LogInformation("Publishing domain event. Event - {event}", domainEvent.GetType().Name);

            await (Task)_busPublishMethod
                .MakeGenericMethod(domainEvent.GetType())
                .Invoke(_bus, new object[] { domainEvent, default(CancellationToken) })!;
        }
    }
}