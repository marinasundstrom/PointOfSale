using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using MassTransit;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using OrderPriceCalculator;

using Sales.Contracts;
using Sales.Domain.Entities;
using Sales.Infrastructure.Persistence;

namespace Sales.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderStatusesController : ControllerBase
    {
        private readonly ILogger<OrderStatusesController> _logger;
        private readonly SalesContext context;

        public OrderStatusesController(
            ILogger<OrderStatusesController> logger,
            SalesContext context)
        {
            _logger = logger;
            this.context = context;
        }

        /// <summary>
        /// Get Order Statuses
        /// </summary>
        [HttpGet]
        public async Task<IEnumerable<OrderStatusDto>> GetStatuses()
        {
            var orderStatuses = await context.OrderStatuses.ToListAsync();
            return orderStatuses.Select(Mappings.CreateOrderStatusDto);
        }
    }
}