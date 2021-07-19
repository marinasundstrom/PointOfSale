using System;
using System.Linq;
using System.Threading.Tasks;

using MassTransit;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Catalog.Contracts;
using Catalog.Domain.Entities;
using Catalog.Infrastructure.Persistence;

namespace Catalog.Application
{
    public class AddCustomFieldToItemCommandHandler : IConsumer<AddCustomFieldToItemCommand>
    {
        private readonly ILogger<AddCustomFieldToItemCommandHandler> _logger;
        private readonly CatalogContext context;
        private readonly IBus bus;

        public AddCustomFieldToItemCommandHandler(
            ILogger<AddCustomFieldToItemCommandHandler> logger,
            CatalogContext context,
            IBus bus)
        {
            _logger = logger;
            this.context = context;
            this.bus = bus;
        }

        public async Task Consume(ConsumeContext<AddCustomFieldToItemCommand> consumeContext)
        {
            var message = consumeContext.Message;

            var details = message.CreateCustomFieldDetails;

            var order = await context.Items
                 .Where(c => c.ItemId == message.ItemId)
                 .FirstOrDefaultAsync();

            if (order is null)
            {
                throw new Exception();
            }

            var customField = new CustomField
            {
                Id = Guid.NewGuid(),
                CustomFieldId = details.CustomFieldId,
                Value = details.Value
            };

            order.CustomFields.Add(customField);

            context.CustomFields.Add(customField);

            await context.SaveChangesAsync();

            await consumeContext.RespondAsync<AddCustomFieldToItemCommandResponse>(new AddCustomFieldToItemCommandResponse());
        }
    }

    public class RemoveCustomFieldFromItemCommandHandler : IConsumer<RemoveCustomFieldFromItemCommand>
    {
        private readonly ILogger<RemoveCustomFieldFromItemCommandHandler> _logger;
        private readonly CatalogContext context;
        private readonly IBus bus;

        public RemoveCustomFieldFromItemCommandHandler(
            ILogger<RemoveCustomFieldFromItemCommandHandler> logger,
            CatalogContext context,
            IBus bus)
        {
            _logger = logger;
            this.context = context;
            this.bus = bus;
        }

        public async Task Consume(ConsumeContext<RemoveCustomFieldFromItemCommand> consumeContext)
        {
            var message = consumeContext.Message;

            var order = await context.Items
                .Where(c => c.ItemId == message.ItemId)
                .FirstOrDefaultAsync();

            if (order is null)
            {
                throw new Exception();
            }

            var customField = order.CustomFields.FirstOrDefault(x => x.CustomFieldId == message.CustomFieldId);

            if (customField is null)
            {
                throw new Exception();
            }

            order.CustomFields.Remove(customField);

            context.CustomFields.Remove(customField);

            await context.SaveChangesAsync();

            await consumeContext.RespondAsync<RemoveCustomFieldFromItemCommandResponse>(new RemoveCustomFieldFromItemCommandResponse());
        }
    }
}