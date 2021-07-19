namespace Customers.Application.Persons;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Customers.Contracts.Persons;
using Customers.Infrastructure.Persistence;

using MassTransit;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

public class GetPersonsQueryHandler : IConsumer<GetPersonsQuery>
{
    private readonly ILogger<GetPersonsQueryHandler> _logger;
    private readonly CustomersContext context;

    public GetPersonsQueryHandler(
        ILogger<GetPersonsQueryHandler> logger,
        CustomersContext context)
    {
        _logger = logger;
        this.context = context;
    }

    public async Task Consume(ConsumeContext<GetPersonsQuery> consumeContext)
    {
        var message = consumeContext.Message;

        var query = context.Persons
                .Include(p => p.Addresses)
                .ThenInclude(p => p.Type)
                .Include(p => p.Organization)
                .Include(p => p.CustomFields)
                .AsSplitQuery()
                .AsNoTracking();

        var total = await query.CountAsync();

        var items = await query
            .Skip(message.Skip)
            .Take(message.Limit)
            .ToArrayAsync();

        var response = new GetPersonsQueryResponse()
        {
            Persons = items.Select(Mappings.ToDto),
            Total = total
        };

        await consumeContext.RespondAsync<GetPersonsQueryResponse>(response);
    }
}

public class GetPersonByCustomerNoQueryHandler : IConsumer<GetPersonByCustomerNoQuery>
{
    private readonly ILogger<GetPersonByCustomerNoQueryHandler> _logger;
    private readonly CustomersContext context;

    public GetPersonByCustomerNoQueryHandler(
        ILogger<GetPersonByCustomerNoQueryHandler> logger,
        CustomersContext context)
    {
        _logger = logger;
        this.context = context;
    }

    public async Task Consume(ConsumeContext<GetPersonByCustomerNoQuery> consumeContext)
    {
        var message = consumeContext.Message;

        var person = await context.Persons
            .Include(p => p.Addresses)
            .ThenInclude(p => p.Type)
            .Include(p => p.Organization)
            .Include(p => p.CustomFields)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == message.PersonId);

        if (person is null)
        {
            throw new Exception();
        }

        var dto = Mappings.ToDto(person);

        await consumeContext.RespondAsync<PersonDto>(dto);
    }
}