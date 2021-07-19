using Billing.Contracts;

using Carts.Contracts;
//using Carts.Client;

using Catalog.Contracts;

using Checkout.Controllers;
using Checkout.Infrastructure.Persistence;

using Marketing.Contracts;

using MassTransit;

using Microsoft.Extensions.DependencyInjection;

using Payments.Contracts;

using Sales.Contracts;

using Warehouse.Contracts;

namespace Checkout.Application.Services
{
    public class CheckoutFinalizationService
    {
        private readonly IRequestClient<GetCatalogItemsByIdQuery> getCatalogItemsByItemIdQueryClient;
        //private readonly ICartClient cartClient;
        private readonly IRequestClient<GetOrderByOrderNoQuery> getOrderByOrderNoQueryClient;
        private readonly IRequestClient<CreateOrderCommand> createOrderCommandClient;
        private readonly IRequestClient<CreateReceiptCommand> createReceiptCommandClient;
        private readonly IRequestClient<MakePaymentRequestCommand> makePaymentRequestCommandClient;
        private readonly IRequestClient<ReserveWarehouseItemCommand> reserveWarehouseItemCommandClient;

        public CheckoutFinalizationService(
            IRequestClient<GetCatalogItemsByIdQuery> getCatalogItemsByItemIdQueryClient,
            //ICartClient cartClient,
            IRequestClient<GetOrderByOrderNoQuery> getOrderByOrderNoQueryClient,
            IRequestClient<CreateOrderCommand> createOrderCommandClient,
            IRequestClient<CreateReceiptCommand> createReceiptCommandClient,
            IRequestClient<MakePaymentRequestCommand> makePaymentRequestCommandClient,
            IRequestClient<ReserveWarehouseItemCommand> reserveWarehouseItemCommandClient)
        {
            this.getCatalogItemsByItemIdQueryClient = getCatalogItemsByItemIdQueryClient;
            //this.cartClient = cartClient;
            this.getOrderByOrderNoQueryClient = getOrderByOrderNoQueryClient;
            this.createOrderCommandClient = createOrderCommandClient;
            this.createReceiptCommandClient = createReceiptCommandClient;
            this.makePaymentRequestCommandClient = makePaymentRequestCommandClient;
            this.reserveWarehouseItemCommandClient = reserveWarehouseItemCommandClient;
        }

        public async Task<int> Initialize(CheckoutDto dto, CartDto cart, int? customerNo)
        {
            await ReserveItems(cart);

            var orderNo = await CreateOrder(dto, cart, customerNo);

            if (dto.PaymentMethod == "Swish")
            {
                await makePaymentRequestCommandClient.GetResponse<MakePaymentRequestCommandResponse>(new MakePaymentRequestCommand()
                {
                    PaymentMethod = "Swish",
                    Data = "0323434534",
                    AmountRequested = cart.Total,
                    Due = DateTime.Now.AddMinutes(5),
                    OrderRef = orderNo.ToString(),
                });
            }

            return orderNo;
        }

        private async Task ReserveItems(CartDto cart)
        {
            var response = await getCatalogItemsByItemIdQueryClient.GetResponse<GetCatalogItemsByIdQueryResponse>(new GetCatalogItemsByIdQuery()
            {
                Ids = cart.Items.Select(i => i.ItemId.ToString())
            });

            var catalogItems = response.Message.Items;

            foreach (var cartItem in cart.Items)
            {
                var catalogItem = catalogItems.FirstOrDefault(x => x.Id == cartItem.ItemId);

                if (catalogItem is null)
                {
                    continue;
                }

                await reserveWarehouseItemCommandClient.GetResponse<ReserveWarehouseItemCommandResponse>(
                    new ReserveWarehouseItemCommand()
                    {
                        ItemId = catalogItem.Id,
                        QuantityReserved = (int)cartItem.Quantity
                    }
                );
            }
        }

        public async Task<int> CreateOrder(CheckoutDto dto, CartDto cart, int? customerNo)
        {
            var response = await getCatalogItemsByItemIdQueryClient.GetResponse<GetCatalogItemsByIdQueryResponse>(new GetCatalogItemsByIdQuery()
            {
                Ids = cart.Items.Select(i => i.ItemId!.ToString())
            });

            var catalogItems = response.Message.Items;

            var cfs = new Dictionary<string, string>();

            if (customerNo != null)
            {
                cfs.Add("customerNo", customerNo.ToString()!);
            }

            var request = new CreateOrderDto()
            {
                Status = "draft",
                Discounts = cart.Discounts.Select(i => new OrderDiscountDto()
                {
                    Amount = i.Amount,
                    Percent = i.Percent,
                    Quantity = i.Quantity,
                    Description = i.Description,
                    DiscountId = i.DiscountId
                }).ToList(),
                Charges = cart.Charges.Select(i => new OrderChargeDto()
                {
                    Amount = i.Amount,
                    Percent = i.Percent,
                    Quantity = i.Quantity,
                    Limit = i.Limit,
                    Description = i.Description,
                    ChargeId = i.ChargeId
                }).ToList(),
                CustomFields = cfs
            }; ;

            request.Items = cart!.Items.Select(i =>
            {
                var item = new CreateOrderItemDto()
                {
                    Description = i.Description,
                    ItemId = i.ItemId!,
                    Quantity = i.Quantity,
                    Discounts = i.Discounts.Select(d => new OrderDiscountDto()
                    {
                        Amount = d.Amount,
                        Percent = d.Percent,
                        Total = d.Total,
                        Quantity = d.Quantity,
                        Limit = d.Limit,
                        Description = d.Description,
                        DiscountId = d.DiscountId
                    }).ToList(),
                    Charges = i.Charges.Select(c => new OrderChargeDto()
                    {
                        Amount = c.Amount,
                        Percent = c.Percent,
                        Total = c.Total,
                        Quantity = c.Quantity,
                        Limit = c.Limit,
                        Description = c.Description,
                        ChargeId = c.ChargeId
                    }).ToList()
                };

                var catalogItem = catalogItems.First(p => p.Id == i.ItemId);

                if (catalogItem.CustomFields.TryGetValue("compareAtPrice", out var compareAtPrice))
                {
                    item.CustomFields = new Dictionary<string, string>();
                    item.CustomFields.Add("regularPrice", compareAtPrice.ToString()!);
                }

                return item;
            }).ToList();

            var response2 = await createOrderCommandClient.GetResponse<CreateOrderCommandResponse>(new CreateOrderCommand
            {
                Dto = request
            });

            return response2.Message.OrderNo;
        }

        public async Task Finalize(int orderNo)
        {
            await CreateReceipt(orderNo);

            //var cartId = await cartClient.ClearCartAsync(dto.CartId);

            //await cartClient.ClearCartAsync(dto.CartId);
        }

        private async Task CreateReceipt(int orderNo)
        {
            var response = await getOrderByOrderNoQueryClient.GetResponse<OrderDto>(new GetOrderByOrderNoQuery
            {
                OrderNo = orderNo
            });

            var order = response.Message;

            var response2 = await getCatalogItemsByItemIdQueryClient.GetResponse<GetCatalogItemsByIdQueryResponse>(new GetCatalogItemsByIdQuery()
            {
                Ids = order.Items.Select(i => i.ItemId!.ToString())
            });

            var catalogItems = response2.Message.Items;

            var cfs = new Dictionary<string, string>() {
                { "orderNo", order.OrderNo.ToString() }
            };

            if (order.CustomFields.TryGetValue("customerNo", out var customerNo))
            {
                cfs.Add("customerNo", customerNo.ToString()!);
            }

            var request = new CreateReceiptDto()
            {
                //Cashier = "Lotta",
                //CheckoutPoint = "Test 1",
                //CustomerNo = dto.CustomerNo.ToString(),
                Discounts = order.Discounts.Select(i => new ReceiptDiscountDto()
                {
                    Amount = i.Amount,
                    Percent = i.Percent,
                    Quantity = i.Quantity,
                    Description = i.Description,
                    DiscountId = i.DiscountId
                }).ToList(),
                Charges = order.Charges.Select(i => new ReceiptChargeDto()
                {
                    Amount = i.Amount,
                    Percent = i.Percent,
                    Quantity = i.Quantity,
                    Limit = i.Limit,
                    Description = i.Description,
                    ChargeId = i.ChargeId
                }).ToList(),
                /*
                Payment = new PaymentDetailsDto
                {
                    Paid = dto?.Amount
                },
                Signature = dto?.Signature,
                */
                CustomFields = cfs
            };

            request.Items = order!.Items.Select(i =>
            {
                var item = new CreateReceiptItemDto()
                {
                    Description = i.Description,
                    ItemId = i.ItemId,
                    Unit = i.Unit!.Code,
                    VatRate = i.VatRate,
                    Price = i.Price,
                    Quantity = i.Quantity,
                    Discounts = i.Discounts.Select(d => new ReceiptDiscountDto()
                    {
                        Amount = d.Amount,
                        Percent = d.Percent,
                        Quantity = d.Quantity,
                        Description = d.Description,
                        DiscountId = d.DiscountId
                    }).ToList(),
                    Charges = i.Charges.Select(c => new ReceiptChargeDto()
                    {
                        Amount = c.Amount,
                        Percent = c.Percent,
                        Total = c.Total,
                        Quantity = c.Quantity,
                        Limit = c.Limit,
                        Description = c.Description,
                        ChargeId = c.ChargeId
                    }).ToList()
                };

                var catalogItem = catalogItems.First(p => p.Id == i.ItemId);

                if (catalogItem.CustomFields.TryGetValue("compareAtPrice", out var compareAtPrice))
                {
                    item.CustomFields = new Dictionary<string, string>();
                    item.CustomFields.Add("regularPrice", compareAtPrice.ToString()!);
                }

                return item;
            }).ToList();

            var response3 = await createReceiptCommandClient.GetResponse<ReceiptDto>(new CreateReceiptCommand(request));

            var receipt3 = response.Message;
        }
    }
}