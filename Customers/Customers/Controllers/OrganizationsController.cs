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
    public class OrganizationsController : ControllerBase
    {
        private readonly ILogger<OrganizationsController> _logger;
        private readonly IBus bus;
        private readonly CustomersContext context;

        public OrganizationsController(ILogger<OrganizationsController> logger, IBus bus, CustomersContext context)
        {
            _logger = logger;
            this.bus = bus;
            this.context = context;
        }

        [HttpGet]
        public async Task<IEnumerable<OrganizationDto>> GetOrganizations()
        {
            var organizations = await context.Organizations
               .Include(p => p.Addresses)
               .ThenInclude(p => p.Type)
               .Include(p => p.Persons)
               .AsNoTracking()
               .ToArrayAsync();

            return organizations.Select(CreateOrganizationDto);
        }

        [HttpGet("{customerNo}")]
        public async Task<OrganizationDto> GetOrganizationByCustomerNo(int customerNo)
        {
            var organization = await context.Organizations
               .Include(p => p.Addresses)
               .ThenInclude(p => p.Type)
                .Include(p => p.Persons)
                .Where(r => r.CustomerNo == customerNo)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (organization is null)
            {
                throw new Exception();
            }

            return CreateOrganizationDto(organization);
        }

        [HttpGet("/GetOrganizationByOrgNo")]
        public async Task<OrganizationDto> GetOrganizationByOrgNo(string orgNO)
        {
            var organization = await context.Organizations
               .Include(p => p.Addresses)
               .ThenInclude(p => p.Type)
                .Include(p => p.Persons)
                .Where(r => r.OrgNo == orgNO)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (organization is null)
            {
                throw new Exception();
            }

            return CreateOrganizationDto(organization);
        }

        private OrganizationDto CreateOrganizationDto(Organization organization)
        {
            return organization.ToDto();
        }
    }
}