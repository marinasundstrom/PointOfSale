using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using MassTransit;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Payments.Contracts;
using Payments.Infrastructure.Persistence;

namespace Payments.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentsController : ControllerBase
    {
        private readonly ILogger<PaymentsController> _logger;
        private readonly PaymentsContext context;

        public PaymentsController(ILogger<PaymentsController> logger, PaymentsContext context)
        {
            _logger = logger;
            this.context = context;
        }

        [HttpGet]
        public async Task<IEnumerable<PaymentDto>> GetPayments([FromServices] IRequestClient<GetPaymentsPaymentsQuery> client)
        {
            var response = await client.GetResponse<GetPaymentsPaymentsQueryResponse>(new GetPaymentsPaymentsQuery());

            return response.Message.Payments;
        }

        [HttpGet("GetPaymentsByOrderRef")]
        public async Task<IEnumerable<PaymentDto>> GetPaymentsByOrderRef([FromServices] IRequestClient<GetPaymentsByOrderRefQuery> client, string orderRef)
        {
            var response = await client.GetResponse<GetPaymentsByOrderRefQueryResponse>(new GetPaymentsByOrderRefQuery(orderRef));

            return response.Message.Payments;
        }

        [HttpGet("{id}")]
        public async Task<PaymentDto> GetPaymentById(Guid id)
        {
            var Payment = await context.Payments
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);

            if (Payment is null)
            {
                throw new Exception();
            }

            return Mappings.ToPaymentDto(Payment);
        }

        [HttpPost("MakePaymentRequest")]
        public async Task MakePaymentRequest([FromServices] IRequestClient<MakePaymentRequestCommand> client, MakePaymentRequestCommand request)
        {
            await client.GetResponse<MakePaymentRequestCommandResponse>(request);
        }

        [HttpPost("{orderRef}/Cancel")]
        public async Task MakePaymentRequest([FromServices] IRequestClient<MakePaymentRequestCommand> client, string orderRef)
        {
            await client.GetResponse<CancelPaymentCommandResponse>(new CancelPaymentCommand
            {
                OrderRef = orderRef
            });
        }
    }
}