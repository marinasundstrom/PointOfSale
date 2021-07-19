using MassTransit;

using Microsoft.AspNetCore.Mvc;

using global::Customers.Contracts;
using global::Customers.Contracts.Persons;

namespace WebApi.Customers;

[ApiController]
[Route("Customers/Persons")]
[Produces("application/prs.hal-forms+json")]
public class CustomersPersonsController : ControllerBase
{
    private readonly ILogger<CustomersPersonsController> _logger;
    private readonly WebApi.Customers.Mapper mapper;

    public CustomersPersonsController(ILogger<CustomersPersonsController> logger, WebApi.Customers.Mapper mapper)
    {
        _logger = logger;
        this.mapper = mapper;
    }

    [HttpGet]
    public async Task<WebApi.Customers.Persons> GetPersons(
        [FromServices] WebApi.Hal.Mapper halMapper,
        [FromServices] IRequestClient<GetPersonsQuery> client,
        int skip = 0, int limit = 10, [FromQuery] string[] embed = null!)
    {
        var response = await client.GetResponse<GetPersonsQueryResponse>(new GetPersonsQuery()
        {
            Skip = skip,
            Limit = limit,
            IncludeCustomFields = embed.Contains("all") || embed.Contains("customFields")
        });

        var message = response.Message;

        var r = mapper.Map(message.Persons, embed);

        r.Count = message.Persons.Count();
        r.Total = message.Total;

        return halMapper.Append("/customers/persons", r, skip, limit, embed, (skip + r.Count) < r.Total);
    }

    [HttpGet("{personId}")]
    public async Task<WebApi.Customers.Person> GetPersonByCustomerNo(
        [FromServices] IRequestClient<GetPersonByCustomerNoQuery> client,
        Guid personId, [FromQuery] string[] embed)
    {
        var response = await client.GetResponse<PersonDto>(new GetPersonByCustomerNoQuery()
        {
            PersonId = personId,
            IncludeCustomFields = embed.Contains("all") || embed.Contains("customFields")
        });

        return mapper.Map(response.Message, embed);
    }

     /// <summary>
    /// Add Custom Field to Person
    /// </summary>
    [HttpPost("{personId}/CustomFields")]
    public async Task AddCustomFieldToPerson(
        [FromServices] IRequestClient<AddCustomFieldToPersonCommand> client, 
        Guid personId, [FromBody] WebApi.Shared.CreateOrUpdateCustomField details)
    {
        await client.GetResponse<AddCustomFieldToPersonCommandResponse>(
            new AddCustomFieldToPersonCommand(personId, new CreateCustomFieldDetails {
                CustomFieldId = details.Name,
                Value = details.Value.ToString()!
            })
        );
    }

    /// <summary>
    /// Remove Custom Field from Person
    /// </summary>
    [HttpDelete("{personId}/CustomFields/{customFieldId}")]
    public async Task RemoveCustomFieldFromPerson(
        [FromServices] IRequestClient<RemoveCustomFieldFromPersonCommand> client,
        Guid personId, string customFieldId)
    {
        await client.GetResponse<RemoveCustomFieldFromPersonCommandResponse>(new RemoveCustomFieldFromPersonCommand(personId, customFieldId));
    }
}