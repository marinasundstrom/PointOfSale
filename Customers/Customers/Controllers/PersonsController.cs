using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Customers.Domain.Entities;
using Customers.Infrastructure.Persistence;

using MassTransit;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Customers.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PersonsController : ControllerBase
    {
        private readonly ILogger<PersonsController> _logger;
        private readonly IBus bus;
        private readonly CustomersContext context;

        public PersonsController(ILogger<PersonsController> logger, IBus bus, CustomersContext context)
        {
            _logger = logger;
            this.bus = bus;
            this.context = context;
        }

        [HttpGet]
        public async Task<IEnumerable<PersonDto>> GetPersons()
        {
            var persons = await context.Persons
               .Include(p => p.Addresses)
               .ThenInclude(p => p.Type)
               .Include(p => p.Organization)
               .AsNoTracking()
               .ToArrayAsync();

            return persons.Select(CreatePersonDto);
        }

        [HttpGet("{customerNo}")]
        public async Task<PersonDto> GetPersonByCustomerNo(int customerNo)
        {
            var person = await context.Persons
               .Include(p => p.Addresses)
               .ThenInclude(p => p.Type)
               .Include(p => p.Organization)
                .Where(r => r.CustomerNo == customerNo)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (person is null)
            {
                throw new Exception();
            }

            return CreatePersonDto(person);
        }

        [HttpGet("/GetPersonBySsn")]
        public async Task<PersonDto> GetPersonBySsn(string ssn)
        {
            var person = await context.Persons
                .Include(p => p.Addresses)
               .ThenInclude(p => p.Type)
                .Include(p => p.Organization)
                .Where(r => r.Ssn == ssn)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (person is null)
            {
                throw new Exception();
            }

            return CreatePersonDto(person);
        }

        private PersonDto CreatePersonDto(Person person)
        {
            return person.ToDto();
        }
    }
}