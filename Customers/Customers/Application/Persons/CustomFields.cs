using System;
using System.Linq;
using System.Threading.Tasks;

using MassTransit;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Customers.Contracts;
using Customers.Contracts.Persons;
using Customers.Domain.Entities;
using Customers.Infrastructure.Persistence;

namespace Customers.Application.Persons
{
    public class AddCustomFieldToPersonCommandHandler : IConsumer<AddCustomFieldToPersonCommand>
    {
        private readonly ILogger<AddCustomFieldToPersonCommandHandler> _logger;
        private readonly CustomersContext context;
        private readonly IBus bus;

        public AddCustomFieldToPersonCommandHandler(
            ILogger<AddCustomFieldToPersonCommandHandler> logger,
            CustomersContext context,
            IBus bus)
        {
            _logger = logger;
            this.context = context;
            this.bus = bus;
        }

        public async Task Consume(ConsumeContext<AddCustomFieldToPersonCommand> consumeContext)
        {
            var message = consumeContext.Message;

            var details = message.CreateCustomFieldDetails;

            var person = await context.Persons
                 .Where(c => c.Id == message.PersonId)
                 .FirstOrDefaultAsync();

            if (person is null)
            {
                throw new Exception();
            }

            var customField = new CustomField
            {
                Id = Guid.NewGuid(),
                CustomFieldId = details.CustomFieldId,
                Value = details.Value
            };

            person.CustomFields.Add(customField);

            context.CustomFields.Add(customField);

            await context.SaveChangesAsync();

            await consumeContext.RespondAsync<AddCustomFieldToPersonCommandResponse>(new AddCustomFieldToPersonCommandResponse());
        }
    }

    public class RemoveCustomFieldFromPersonCommandHandler : IConsumer<RemoveCustomFieldFromPersonCommand>
    {
        private readonly ILogger<RemoveCustomFieldFromPersonCommandHandler> _logger;
        private readonly CustomersContext context;
        private readonly IBus bus;

        public RemoveCustomFieldFromPersonCommandHandler(
            ILogger<RemoveCustomFieldFromPersonCommandHandler> logger,
            CustomersContext context,
            IBus bus)
        {
            _logger = logger;
            this.context = context;
            this.bus = bus;
        }

        public async Task Consume(ConsumeContext<RemoveCustomFieldFromPersonCommand> consumeContext)
        {
            var message = consumeContext.Message;

            var person = await context.Persons
                .Where(c => c.Id == message.PersonId)
                .FirstOrDefaultAsync();

            if (person is null)
            {
                throw new Exception();
            }

            var customField = person.CustomFields.FirstOrDefault(x => x.CustomFieldId == message.CustomFieldId);

            if (customField is null)
            {
                throw new Exception();
            }

            person.CustomFields.Remove(customField);

            context.CustomFields.Remove(customField);

            await context.SaveChangesAsync();

            await consumeContext.RespondAsync<RemoveCustomFieldFromPersonCommandResponse>(new RemoveCustomFieldFromPersonCommandResponse());
        }
    }
}