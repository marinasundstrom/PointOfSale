using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Billing.Client;

using Carts.Client;

using Catalog.Client;

using Checkout.Application.Services;
using Checkout.Infrastructure.Persistence;

using Customers.Client;

using Marketing.Client;

using MassTransit;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Payments.Client;

using Sales.Client;

using Warehouse.Contracts;

namespace Checkout.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CheckoutController : ControllerBase
    {
        private readonly ILogger<CheckoutController> _logger;
        private readonly IBus bus;
        private readonly CheckoutContext context;
        private readonly IItemsClient catalogItemsClient;
        private readonly ICartClient cartClient;
        private readonly IOrdersClient ordersClient;
        private readonly IReceiptsClient receiptsClient;
        private readonly IPersonsClient personsClient;
        private readonly IDiscountsClient discountsClient;
        private readonly IPaymentsClient paymentsClient;
        private readonly IRequestClient<Carts.Contracts.GetCartByIdQuery> getCartByIdQueryClient;
        private readonly IRequestClient<ReserveWarehouseItemCommand> reserveWarehouseItemCommandClient;
        private readonly CheckoutFinalizationService checkoutFinalizationService;

        public CheckoutController(
            ILogger<CheckoutController> logger,
            IBus bus,
            CheckoutContext context,
            IItemsClient catalogItemsClient,
            ICartClient cartClient,
            IOrdersClient ordersClient,
            IReceiptsClient receiptsClient,
            IPersonsClient personsClient,
            IDiscountsClient discountsClient,
            IPaymentsClient paymentsClient,
            IRequestClient<Carts.Contracts.GetCartByIdQuery> getCartByIdQueryClient,
            IRequestClient<ReserveWarehouseItemCommand> reserveWarehouseItemCommandClient,
            CheckoutFinalizationService checkoutFinalizationService)
        {
            _logger = logger;
            this.bus = bus;
            this.context = context;
            this.catalogItemsClient = catalogItemsClient;
            this.cartClient = cartClient;
            this.ordersClient = ordersClient;
            this.receiptsClient = receiptsClient;
            this.personsClient = personsClient;
            this.discountsClient = discountsClient;
            this.paymentsClient = paymentsClient;
            this.getCartByIdQueryClient = getCartByIdQueryClient;
            this.reserveWarehouseItemCommandClient = reserveWarehouseItemCommandClient;
            this.checkoutFinalizationService = checkoutFinalizationService;
        }

        [HttpPost]
        public async Task Checkout(CheckoutDto dto)
        {
            var response = await getCartByIdQueryClient.GetResponse<Carts.Contracts.CartDto>(new Carts.Contracts.GetCartByIdQuery
            {
                Id = dto.CartId
            });

            var cart = response.Message;

            int? customerNo = null;

            if (!string.IsNullOrEmpty(cart.Tag) && cart.Tag.StartsWith("customer-"))
            {
                customerNo = int.Parse(cart.Tag.Substring(cart.Tag.IndexOf("-") + 1));
            }

            var orderNo = await checkoutFinalizationService.Initialize(dto, cart, customerNo);

            if (dto.PaymentMethod != "Swish")
            {
                await checkoutFinalizationService.Finalize(orderNo);
            }
        }

        [HttpGet("Carts")]
        public async Task<IEnumerable<CheckoutCartDto>> GetCartsAsync()
        {
            var carts = await cartClient.GetCartsAsync();
            var persons = await personsClient.GetPersonsAsync();

            return carts.Select(c =>
            {
                return new CheckoutCartDto()
                {
                    Id = c.Id,
                    Tag = c.Tag,
                    Customer = c.Tag is not null ? CreateDto(persons
                        .FirstOrDefault(x => x.CustomerNo == int.Parse(c.Tag.Replace("customer-", string.Empty)))) : null,
                    Items = c.Items.Select(i => new CheckoutCartItemDto()
                    {
                        Id = i.Id,
                        Description = i.Description,
                        Price = i.Price,
                        ItemId = i.ItemId.ToString(),
                        Quantity = i.Quantity,
                        Total = i.Total,
                        Unit = i.Unit,
                        VatRate = i.VatRate
                    }),
                    Vat = c.Totals.ToDictionary(x => x.Key, x => new CheckoutVatSummaryDto(x.Value.SubTotal, x.Value.Vat, x.Value.Total)),
                    Rounding = c.Rounding,
                    Total = c.Total
                };
            })
            .OrderBy(x => x.Tag);
        }

        private CheckoutPersonDto? CreateDto(PersonDto? personDto)
        {
            if (personDto is null)
                return null;

            return new CheckoutPersonDto()
            {
                Id = personDto.Id,
                CustomerNo = personDto.CustomerNo.GetValueOrDefault(),
                FirstName = personDto.FirstName,
                LastName = personDto.LastName,
                Ssn = personDto.Ssn,
            };
        }

        [HttpPost("Return")]
        public async Task Return(CheckoutReturnDto dto)
        {
            var receipt = await receiptsClient.GetReceiptByNoAsync(dto.ReceiptNo, new string[] {
                "items", "discounts", "charges"
            });

            if (receipt.Type == ReceiptType.Return)
            {
                throw new Exception("Returns cannot be returned.");
            }

            List<CreateReceiptItemDto> newRecipeItemDtos = new();

            foreach (var item in dto.Items)
            {
                var receiptItem = receipt._embedded.Items.FirstOrDefault(x => x.Id.ToString() == item.ItemId);

                if (receiptItem is null)
                {
                    throw new Exception("Item not found");
                }

                if (item.Quantity == 0)
                {
                    throw new Exception("Quantity cannot be 0");
                }

                if (item.Quantity > receiptItem.Quantity)
                {
                    throw new Exception("Quantity must not be greater than the number of goods sold.");
                }

                newRecipeItemDtos.Add(new CreateReceiptItemDto()
                {
                    Description = receiptItem.Description,
                    ItemId = receiptItem.ItemId,
                    Unit = receiptItem.Unit,
                    Quantity = item.Quantity,
                    Price = receiptItem.Price,
                    VatRate = receiptItem.VatRate
                });
            }

            var request = new CreateReceiptDto()
            {
                ReceiptType = ReceiptType.Return,
                Cashier = "Lotta",
                CheckoutPoint = "Test 1",
                CustomerNo = receipt.AdditionalProperties.FirstOrDefault(x => x.Key == "customerNo").Value?.ToString(),
                Items = newRecipeItemDtos,
                Signature = dto?.Signature
            };

            var newReceipt = await receiptsClient.CreateReceiptAsync(request);
        }

        static SwishApi.Client CreateClient(bool useLocalCertificate = true)
        {
            var callbackUri = "https://tabetaltmedswish.se/Test/Callback/";

            SwishApi.Client client;

            if (useLocalCertificate)
            {
                // Get the path for the test certificate in the TestCert folder in the console application folder, being always copy to the output folder
                string certificatePath = Environment.CurrentDirectory + "\\TestCert\\Swish_Merchant_TestCertificate_1234679304.p12";

                // Create a Swishpi Client object with all data needed to run a test Swish payment
                client = new SwishApi.Client(certificatePath, "swish", callbackUri);
            }
            else
            {
                // In an architecture where we have an upstream/proxy that manage certificates,
                // we can construct a client that don't pass a local certificate in the request.
                //
                // In a context like that, the URI is also not the URI to the Swish server, but
                // but rather the URI of the proxy.

                client = new SwishApi.Client(callbackUri, "https://my_certificate_proxy.corp");
            }

            // client.EnableHTTPLog = true;
            return client;
        }
    }
}