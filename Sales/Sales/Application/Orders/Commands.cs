using System;
using System.Linq;
using System.Threading.Tasks;

using Catalog.Client;

using MassTransit;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using OrderPriceCalculator;

using Sales.Application.Subscriptions;
using Sales.Contracts;
using Sales.Domain.Entities;
using Sales.Infrastructure.Persistence;

namespace Sales.Application.Orders
{
    public class CreateOrderCommandHandler : IConsumer<CreateOrderCommand>
    {
        private readonly ILogger<CreateOrderCommandHandler> _logger;
        private readonly IItemsClient catalogItemsClient;
        private readonly SalesContext context;
        private readonly IBus bus;

        public CreateOrderCommandHandler(
            ILogger<CreateOrderCommandHandler> logger,
            IItemsClient catalogItemsClient,
            SalesContext context,
            IBus bus)
        {
            _logger = logger;
            this.catalogItemsClient = catalogItemsClient;
            this.context = context;
            this.bus = bus;
        }

        public async Task Consume(ConsumeContext<CreateOrderCommand> consumeContext)
        {
            var message = consumeContext.Message;

            var order = new Order();
            order.Id = Guid.NewGuid();
            order.OrderDate = DateTime.Now;

            order.StatusId = message?.Dto?.Status ?? "draft";
            order.StatusDate = DateTime.Now;

            try
            {
                order.OrderNo = await context.Orders.MaxAsync(r => r.OrderNo) + 1;
            }
            catch
            {
                order.OrderNo = 1;
            }

            if (message?.Dto != null)
            {
                var dto = message.Dto;

                foreach (var item in dto.Items)
                {
                    await AddItem(order, item);
                }

                if (dto.Charges is not null)
                {
                    foreach (var item in dto.Charges)
                    {
                        AddCharge(order, item);
                    }
                }

                if (dto.Discounts is not null)
                {
                    foreach (var item in dto.Discounts)
                    {
                        AddDiscount(order, item);
                    }
                }

                AddCustomFields(order, dto);
            }

            context.Orders.Add(order);

            order.Update();

            await context.SaveChangesAsync();

            await bus.Publish(new OrderCreatedEvent(order.OrderNo));

            if (order.StatusId == "placed")
            {
                await bus.Publish(new OrderCreatedEvent(order.OrderNo));
            }

            await consumeContext.RespondAsync<CreateOrderCommandResponse>(new CreateOrderCommandResponse(order.OrderNo));
        }

        private void AddCustomFields(Order order, CreateOrderDto dto)
        {
            if (dto.CustomFields is not null)
            {
                foreach (KeyValuePair<string, string> i in dto.CustomFields)
                {
                    order.CustomFields.Add(new CustomField()
                    {
                        Id = Guid.NewGuid(),
                        CustomFieldId = i.Key,
                        Value = i.Value.ToString()
                    });
                }
            }
        }

        private void AddCustomFields(OrderItem orderItem, CreateOrderItemDto dto)
        {
            if (dto.CustomFields is not null)
            {
                foreach (KeyValuePair<string, string> i in dto.CustomFields)
                {
                    orderItem.CustomFields.Add(new CustomField()
                    {
                        Id = Guid.NewGuid(),
                        CustomFieldId = i.Key,
                        Value = i.Value.ToString()
                    });
                }
            }
        }

        private void AddCharge(Order order, OrderChargeDto chargeDto)
        {
            var charge = new OrderCharge()
            {
                Id = Guid.NewGuid(),
                Order = order,
                Quantity = chargeDto.Quantity,
                Limit = chargeDto.Limit,
                Amount = chargeDto.Amount,
                Percent = chargeDto.Percent,
                Description = chargeDto.Description ?? string.Empty
                //ChargeId = chargeDto.ChargeId
            };

            order.Charges.Add(charge);
            context.OrderCharges.Add(charge);
        }

        private void AddCharge(OrderItem orderItem, OrderChargeDto chargeDto)
        {
            var charge = new OrderCharge()
            {
                Id = Guid.NewGuid(),
                Quantity = chargeDto.Quantity,
                Limit = chargeDto.Limit,
                Amount = chargeDto.Amount,
                Percent = chargeDto.Percent,
                Description = chargeDto.Description!
                //ChargeId = chargeDto.ChargeId
            };

            orderItem.Charges.Add(charge);
        }

        private void AddDiscount(Order order, OrderDiscountDto discountDto)
        {
            var discount = new OrderDiscount()
            {
                Id = Guid.NewGuid(),
                Quantity = discountDto.Quantity,
                Limit = discountDto.Limit,
                Amount = discountDto.Amount,
                Percent = discountDto.Percent,
                Description = discountDto.Description!,
                DiscountId = discountDto.DiscountId
            };

            order.Discounts.Add(discount);
        }

        private void AddDiscount(OrderItem orderItem, OrderDiscountDto discountDto)
        {
            var discount = new OrderDiscount()
            {
                Id = Guid.NewGuid(),
                Quantity = discountDto.Quantity,
                Limit = discountDto.Limit,
                Amount = discountDto.Amount,
                Percent = discountDto.Percent,
                Description = discountDto.Description!,
                DiscountId = discountDto.DiscountId
            };

            orderItem.Discounts.Add(discount);
        }

        private async Task AddItem(Order order, CreateOrderItemDto dto)
        {
            CatalogItemDto? catalogItem = null;

            if (dto.ItemId is not null)
            {
                catalogItem = await catalogItemsClient.GetItemByItemIdAsync(dto.ItemId);
            }

            var orderItem = new OrderItem()
            {
                Id = Guid.NewGuid(),
                ItemId = dto.ItemId!,
                Description = dto.Description ?? catalogItem!.Name,
                Unit = catalogItem!.Unit.Name,
                Quantity = dto.Quantity,
                Price = catalogItem!.VatIncluded ? catalogItem.Price : catalogItem.Price.AddVat(catalogItem.VatRate),
                VatRate = catalogItem!.VatRate
            };

            if (dto.Charges is not null)
            {
                foreach (var d in dto.Charges)
                {
                    AddCharge(orderItem, d);
                }
            }

            if (dto.Discounts is not null)
            {
                foreach (var d in dto.Discounts)
                {
                    AddDiscount(orderItem, d);
                }
            }

            order.Items.Add(orderItem);

            AddCustomFields(orderItem, dto);

            orderItem.Update();

            order.Update();
        }
    }

    public class PlaceOrderCommandHandler : IConsumer<PlaceOrderCommand>
    {
        private readonly ILogger<PlaceOrderCommandHandler> _logger;
        private readonly SalesContext context;
        private readonly IBus bus;
        private readonly SubscriptionOrderGenerator subscriptionOrderGenerator;

        public PlaceOrderCommandHandler(
            ILogger<PlaceOrderCommandHandler> logger,
            SalesContext context,
            IBus bus,
            SubscriptionOrderGenerator subscriptionOrderGenerator)
        {
            _logger = logger;
            this.context = context;
            this.bus = bus;
            this.subscriptionOrderGenerator = subscriptionOrderGenerator;
        }

        public async Task Consume(ConsumeContext<PlaceOrderCommand> consumeContext)
        {
            var message = consumeContext.Message;

            var order = await context.Orders
                .IncludeAll()
                .FirstOrDefaultAsync(c => c.OrderNo == message.OrderNo);

            if (order is null)
            {
                throw new Exception();
            }

            var orders = subscriptionOrderGenerator.GetOrders(order);

            if (orders.Any())
            {
                foreach (var o in orders)
                {
                    o.Update();
                    context.Orders.Add(o);
                }

                await context.SaveChangesAsync();
            }

            await consumeContext.RespondAsync<PlaceOrderCommandResponse>(new PlaceOrderCommandResponse());
        }
    }

    public class UpdateOrderStatusCommandHandler : IConsumer<UpdateOrderStatusCommand>
    {
        private readonly ILogger<UpdateOrderStatusCommandHandler> _logger;
        private readonly SalesContext context;
        private readonly IBus bus;

        public UpdateOrderStatusCommandHandler(
            ILogger<UpdateOrderStatusCommandHandler> logger,
            SalesContext context,
            IBus bus,
            SubscriptionOrderGenerator subscriptionOrderGenerator)
        {
            _logger = logger;
            this.context = context;
            this.bus = bus;
        }

        public async Task Consume(ConsumeContext<UpdateOrderStatusCommand> consumeContext)
        {
            var message = consumeContext.Message;

            var order = await context.Orders
                .IncludeAll()
                .FirstOrDefaultAsync(c => c.OrderNo == message.OrderNo);

            if (order is null)
            {
                throw new Exception();
            }

            order.UpdateOrderStatus(message.OrderStatusId);


            await context.SaveChangesAsync();

            await consumeContext.RespondAsync<UpdateOrderStatusCommandResponse>(new UpdateOrderStatusCommandResponse());
        }
    }

    public class ClearOrderCommandHandler : IConsumer<ClearOrderCommand>
    {
        private readonly ILogger<ClearOrderCommandHandler> _logger;
        private readonly SalesContext context;
        private readonly IBus bus;

        public ClearOrderCommandHandler(
            ILogger<ClearOrderCommandHandler> logger,
            SalesContext context,
            IBus bus)
        {
            _logger = logger;
            this.context = context;
            this.bus = bus;
        }

        public async Task Consume(ConsumeContext<ClearOrderCommand> consumeContext)
        {
            var message = consumeContext.Message;

            var order = await context.Orders
                .IncludeAll()
                .FirstOrDefaultAsync(c => c.OrderNo == message.OrderNo);

            if (order is null)
            {
                throw new Exception();
            }

            order.Clear();

            order.Update();

            await context.SaveChangesAsync();

            await consumeContext.RespondAsync<ClearOrderCommandResponse>(new ClearOrderCommandResponse(order.OrderNo));
        }
    }

    public class AddOrderItemCommandHandler : IConsumer<AddOrderItemCommand>
    {
        private readonly ILogger<AddOrderItemCommandHandler> _logger;
        private readonly SalesContext context;
        private readonly IItemsClient catalogItemsClient;
        private readonly IBus bus;

        public AddOrderItemCommandHandler(
            ILogger<AddOrderItemCommandHandler> logger,
            SalesContext context,
            IItemsClient catalogItemsClient,
            IBus bus)
        {
            _logger = logger;
            this.context = context;
            this.catalogItemsClient = catalogItemsClient;
            this.bus = bus;
        }

        public async Task Consume(ConsumeContext<AddOrderItemCommand> consumeContext)
        {
            var message = consumeContext.Message;

            var order = await context.Orders
                .Where(c => c.OrderNo == message.OrderNo)
                .IncludeAll()
                .FirstOrDefaultAsync();

            if (order is null)
            {
                order = new Order();
                context.Orders.Add(order);
            }

            CatalogItemDto? catalogItem = null;

            if (message.ItemId is not null)
            {
                catalogItem = await catalogItemsClient.GetItemByItemIdAsync(message.ItemId);
            }

            var orderItem = new OrderItem()
            {
                Id = Guid.NewGuid(),
                Order = order,
                Description = message.Description ?? catalogItem!.Name,
                ItemId = message.ItemId!,
                Unit = message.Unit ?? catalogItem!.Unit.Code,
                Quantity = message.Quantity,
                Price = catalogItem!.VatIncluded ? catalogItem.Price : catalogItem.Price.AddVat(catalogItem.VatRate),
                VatRate = catalogItem.VatRate
            };

            order.Items.Add(orderItem);
            context.OrderItems.Add(orderItem);

            order.Update();

            await context.SaveChangesAsync();

            await bus.Publish(new OrderItemAddedEvent(order.OrderNo, orderItem.Id));

            await consumeContext.RespondAsync<OrderItemDto>(Mappings.CreateOrderItemDto(orderItem));
        }
    }

    public class RemoveOrderItemCommandHandler : IConsumer<RemoveOrderItemCommand>
    {
        private readonly ILogger<RemoveOrderItemCommandHandler> _logger;
        private readonly SalesContext context;
        private readonly IBus bus;

        public RemoveOrderItemCommandHandler(
            ILogger<RemoveOrderItemCommandHandler> logger,
            SalesContext context,
            IBus bus)
        {
            _logger = logger;
            this.context = context;
            this.bus = bus;
        }

        public async Task Consume(ConsumeContext<RemoveOrderItemCommand> consumeContext)
        {
            var message = consumeContext.Message;

            var order = context.Orders
                .Where(c => c.OrderNo == message.OrderNo)
                .IncludeAll()
                .FirstOrDefault();

            if (order is null)
            {
                throw new Exception();
            }

            var item = order.Items.FirstOrDefault(i => i.Id == message.OrderItemId);

            if (item is null)
            {
                throw new Exception();
            }

            item.Clear();

            order.Items.Remove(item);

            order.Update();

            await context.SaveChangesAsync();

            item.DomainEvents.Add(new OrderItemRemovedEvent(order.OrderNo, item.Id));

            await consumeContext.RespondAsync<RemoveOrderItemCommandResponse>(new RemoveOrderItemCommandResponse(order.OrderNo, item.Id));
        }
    }

    public class UpdateOrderItemQuantityCommandHandler : IConsumer<UpdateOrderItemQuantityCommand>
    {
        private readonly ILogger<UpdateOrderItemQuantityCommandHandler> _logger;
        private readonly SalesContext context;
        private readonly IBus bus;

        public UpdateOrderItemQuantityCommandHandler(
            ILogger<UpdateOrderItemQuantityCommandHandler> logger,
            SalesContext context,
            IBus bus)
        {
            _logger = logger;
            this.context = context;
            this.bus = bus;
        }

        public async Task Consume(ConsumeContext<UpdateOrderItemQuantityCommand> consumeContext)
        {
            var message = consumeContext.Message;

            var order = context.Orders
                .Where(c => c.OrderNo == message.OrderNo)
                .IncludeAll()
                .FirstOrDefault();

            if (order is null)
            {
                throw new Exception();
            }

            var item = order.Items.FirstOrDefault(i => i.Id == message.OrderItemId);

            if (item is null)
            {
                throw new Exception();
            }

            var oldQuantity = item.Quantity;

            item.UpdateQuantity(message.Quantity);

            order.Update();

            await context.SaveChangesAsync();

            await bus.Publish(new OrderItemQuantityUpdatedEvent(order.OrderNo, item.Id, oldQuantity, item.Quantity));

            await consumeContext.RespondAsync<UpdateOrderItemQuantityCommandResponse>(new UpdateOrderItemQuantityCommandResponse(order.OrderNo, item.Id));
        }
    }

    public class AddDiscountToOrderCommandHandler : IConsumer<AddDiscountToOrderCommand>
    {
        private readonly ILogger<AddDiscountToOrderCommandHandler> _logger;
        private readonly SalesContext context;
        private readonly IBus bus;

        public AddDiscountToOrderCommandHandler(
            ILogger<AddDiscountToOrderCommandHandler> logger,
            SalesContext context,
            IBus bus)
        {
            _logger = logger;
            this.context = context;
            this.bus = bus;
        }

        public async Task Consume(ConsumeContext<AddDiscountToOrderCommand> consumeContext)
        {
            var message = consumeContext.Message;

            var details = message.DiscountDetails;

            var order = await context.Orders
                 .IncludeAll()
                 .Where(c => c.OrderNo == message.OrderNo)
                 .FirstOrDefaultAsync();

            if (order is null)
            {
                throw new Exception();
            }

            if (details.Percent is not null)
            {
                if (order.Discounts.Any(x => x.Percent is null))
                {
                    throw new Exception("Cannot combine different discount types.");
                }

                if (order.Discounts.Any(x => x.Percent is not null))
                {
                    throw new Exception("Cannot add another discount based on percenOrderNoe.");
                }
            }

            if (details.Percent is null && order.Discounts.Any(x => x.Percent is not null))
            {
                throw new Exception("Cannot combine different discount types.");
            }

            var discount = new OrderDiscount
            {
                Id = Guid.NewGuid(),
                Order = order,
                Amount = details.Amount * -1,
                Percent = details.Percent * -1,
                Description = details.Description!,
                DiscountId = details.DiscountId
            };

            order.Discounts.Add(discount);

            context.OrderDiscounts.Add(discount);

            order.Update();

            await context.SaveChangesAsync();

            await consumeContext.RespondAsync<AddDiscountToOrderCommandResponse>(new AddDiscountToOrderCommandResponse());
        }
    }

    public class RemoveDiscountFromOrderCommandHandler : IConsumer<RemoveDiscountFromOrderCommand>
    {
        private readonly ILogger<RemoveDiscountFromOrderCommandHandler> _logger;
        private readonly SalesContext context;
        private readonly IBus bus;

        public RemoveDiscountFromOrderCommandHandler(
            ILogger<RemoveDiscountFromOrderCommandHandler> logger,
            SalesContext context,
            IBus bus)
        {
            _logger = logger;
            this.context = context;
            this.bus = bus;
        }

        public async Task Consume(ConsumeContext<RemoveDiscountFromOrderCommand> consumeContext)
        {
            var message = consumeContext.Message;

            var order = await context.Orders
                .IncludeAll()
                .Where(c => c.OrderNo == message.OrderNo)
                .FirstOrDefaultAsync();

            if (order is null)
            {
                throw new Exception();
            }

            var discount = order.Discounts.FirstOrDefault(x => x.Id == message.DiscountId);

            if (discount is null)
            {
                throw new Exception();
            }

            order.Discounts.Remove(discount);

            context.OrderDiscounts.Remove(discount);

            order.Update();

            await context.SaveChangesAsync();

            await consumeContext.RespondAsync<RemoveDiscountFromOrderCommandResponse>(new RemoveDiscountFromOrderCommandResponse());
        }
    }

    public class AddDiscountToOrderItemCommandHandler : IConsumer<AddDiscountToOrderItemCommand>
    {
        private readonly ILogger<AddDiscountToOrderItemCommandHandler> _logger;
        private readonly SalesContext context;
        private readonly IBus bus;

        public AddDiscountToOrderItemCommandHandler(
            ILogger<AddDiscountToOrderItemCommandHandler> logger,
            SalesContext context,
            IBus bus)
        {
            _logger = logger;
            this.context = context;
            this.bus = bus;
        }

        public async Task Consume(ConsumeContext<AddDiscountToOrderItemCommand> consumeContext)
        {
            var message = consumeContext.Message;

            var details = message.DiscountDetails;

            var order = await context.Orders
                            .IncludeAll()
                            .Where(c => c.OrderNo == message.OrderNo)
                            .FirstOrDefaultAsync();

            if (order is null)
            {
                throw new Exception();
            }

            var orderItem = order.Items.FirstOrDefault(i => i.Id == message.OrderItemId);

            if (orderItem is null)
            {
                throw new Exception();
            }

            if (details.Percent is not null)
            {
                if (orderItem.Discounts.Any(x => x.Percent is null))
                {
                    throw new Exception("Cannot combine different discount types.");
                }

                if (orderItem.Discounts.Any(x => x.Percent is not null))
                {
                    throw new Exception("Cannot add another discount based on percenOrderNoe.");
                }
            }

            if (details.Percent is null && orderItem.Discounts.Any(x => x.Percent is not null))
            {
                throw new Exception("Cannot combine different discount types.");
            }

            var discount = new OrderDiscount
            {
                Id = Guid.NewGuid(),
                OrderItem = orderItem,
                Amount = details.Amount * -1,
                Percent = details.Percent * -1,
                Description = details.Description!,
                DiscountId = details.DiscountId
            };

            orderItem.Discounts.Add(discount);

            context.OrderDiscounts.Add(discount);

            order.Update();

            await context.SaveChangesAsync();

            await consumeContext.RespondAsync<AddDiscountToOrderItemCommandResponse>(new AddDiscountToOrderItemCommandResponse());
        }
    }

    public class RemoveDiscountFromOrderItemCommandHandler : IConsumer<RemoveDiscountFromOrderItemCommand>
    {
        private readonly ILogger<RemoveDiscountFromOrderItemCommandHandler> _logger;
        private readonly SalesContext context;
        private readonly IBus bus;

        public RemoveDiscountFromOrderItemCommandHandler(
            ILogger<RemoveDiscountFromOrderItemCommandHandler> logger,
            SalesContext context,
            IBus bus)
        {
            _logger = logger;
            this.context = context;
            this.bus = bus;
        }

        public async Task Consume(ConsumeContext<RemoveDiscountFromOrderItemCommand> consumeContext)
        {
            var message = consumeContext.Message;

            var order = await context.Orders
             .IncludeAll()
             .Where(c => c.OrderNo == message.OrderNo)
             .FirstOrDefaultAsync();

            if (order is null)
            {
                throw new Exception();
            }

            var orderItem = order.Items.FirstOrDefault(i => i.Id == message.OrderItemId);

            if (orderItem is null)
            {
                throw new Exception();
            }

            var discount = orderItem.Discounts.FirstOrDefault(x => x.Id == message.DiscountId);

            if (discount is null)
            {
                throw new Exception();
            }

            orderItem.Discounts.Remove(discount);

            context.OrderDiscounts.Remove(discount);

            order.Update();

            await context.SaveChangesAsync();

            await consumeContext.RespondAsync<RemoveDiscountFromOrderItemCommandResponse>(new RemoveDiscountFromOrderItemCommandResponse());
        }
    }

    // ---

    public class AddChargeToOrderCommandHandler : IConsumer<AddChargeToOrderCommand>
    {
        private readonly ILogger<AddChargeToOrderCommandHandler> _logger;
        private readonly SalesContext context;
        private readonly IBus bus;

        public AddChargeToOrderCommandHandler(
            ILogger<AddChargeToOrderCommandHandler> logger,
            SalesContext context,
            IBus bus)
        {
            _logger = logger;
            this.context = context;
            this.bus = bus;
        }

        public async Task Consume(ConsumeContext<AddChargeToOrderCommand> consumeContext)
        {
            var message = consumeContext.Message;

            var details = message.ChargeDetails;

            var order = await context.Orders
                 .IncludeAll()
                 .Where(c => c.OrderNo == message.OrderNo)
                 .FirstOrDefaultAsync();

            if (order is null)
            {
                throw new Exception();
            }

            if (details.Percent is not null)
            {
                if (order.Charges.Any(x => x.Percent is null))
                {
                    throw new Exception("Cannot combine different Charge types.");
                }

                if (order.Charges.Any(x => x.Percent is not null))
                {
                    throw new Exception("Cannot add another Charge based on percenOrderNoe.");
                }
            }

            if (details.Percent is null && order.Charges.Any(x => x.Percent is not null))
            {
                throw new Exception("Cannot combine different Charge types.");
            }

            var Charge = new OrderCharge
            {
                Id = Guid.NewGuid(),
                Order = order,
                Amount = details.Amount,
                Percent = details.Percent,
                Description = details.Description!,
                ChargeId = details.ChargeId
            };

            order.Charges.Add(Charge);

            context.OrderCharges.Add(Charge);

            order.Update();

            await context.SaveChangesAsync();

            await consumeContext.RespondAsync<AddChargeToOrderCommandResponse>(new AddChargeToOrderCommandResponse());
        }
    }

    public class RemoveChargeFromOrderCommandHandler : IConsumer<RemoveChargeFromOrderCommand>
    {
        private readonly ILogger<RemoveChargeFromOrderCommandHandler> _logger;
        private readonly SalesContext context;
        private readonly IBus bus;

        public RemoveChargeFromOrderCommandHandler(
            ILogger<RemoveChargeFromOrderCommandHandler> logger,
            SalesContext context,
            IBus bus)
        {
            _logger = logger;
            this.context = context;
            this.bus = bus;
        }

        public async Task Consume(ConsumeContext<RemoveChargeFromOrderCommand> consumeContext)
        {
            var message = consumeContext.Message;

            var order = await context.Orders
                .IncludeAll()
                .Where(c => c.OrderNo == message.OrderNo)
                .FirstOrDefaultAsync();

            if (order is null)
            {
                throw new Exception();
            }

            var Charge = order.Charges.FirstOrDefault(x => x.Id == message.ChargeId);

            if (Charge is null)
            {
                throw new Exception();
            }

            order.Charges.Remove(Charge);

            context.OrderCharges.Remove(Charge);

            order.Update();

            await context.SaveChangesAsync();

            await consumeContext.RespondAsync<RemoveChargeFromOrderCommandResponse>(new RemoveChargeFromOrderCommandResponse());
        }
    }

    public class AddChargeToOrderItemCommandHandler : IConsumer<AddChargeToOrderItemCommand>
    {
        private readonly ILogger<AddChargeToOrderItemCommandHandler> _logger;
        private readonly SalesContext context;
        private readonly IBus bus;

        public AddChargeToOrderItemCommandHandler(
            ILogger<AddChargeToOrderItemCommandHandler> logger,
            SalesContext context,
            IBus bus)
        {
            _logger = logger;
            this.context = context;
            this.bus = bus;
        }

        public async Task Consume(ConsumeContext<AddChargeToOrderItemCommand> consumeContext)
        {
            var message = consumeContext.Message;

            var details = message.ChargeDetails;

            var order = await context.Orders
                            .IncludeAll()
                            .Where(c => c.OrderNo == message.OrderNo)
                            .FirstOrDefaultAsync();

            if (order is null)
            {
                throw new Exception();
            }

            var orderItem = order.Items.FirstOrDefault(i => i.Id == message.OrderItemId);

            if (orderItem is null)
            {
                throw new Exception();
            }

            if (details.Percent is not null)
            {
                if (orderItem.Charges.Any(x => x.Percent is null))
                {
                    throw new Exception("Cannot combine different Charge types.");
                }

                if (orderItem.Charges.Any(x => x.Percent is not null))
                {
                    throw new Exception("Cannot add another Charge based on percenOrderNoe.");
                }
            }

            if (details.Percent is null && orderItem.Charges.Any(x => x.Percent is not null))
            {
                throw new Exception("Cannot combine different Charge types.");
            }

            var Charge = new OrderCharge
            {
                Id = Guid.NewGuid(),
                OrderItem = orderItem,
                Amount = details.Amount,
                Percent = details.Percent,
                Description = details.Description!,
                ChargeId = details.ChargeId
            };

            orderItem.Charges.Add(Charge);

            context.OrderCharges.Add(Charge);

            order.Update();

            await context.SaveChangesAsync();

            await consumeContext.RespondAsync<AddChargeToOrderItemCommandResponse>(new AddChargeToOrderItemCommandResponse());
        }
    }

    public class RemoveChargeFromOrderItemCommandHandler : IConsumer<RemoveChargeFromOrderItemCommand>
    {
        private readonly ILogger<RemoveChargeFromOrderItemCommandHandler> _logger;
        private readonly SalesContext context;
        private readonly IBus bus;

        public RemoveChargeFromOrderItemCommandHandler(
            ILogger<RemoveChargeFromOrderItemCommandHandler> logger,
            SalesContext context,
            IBus bus)
        {
            _logger = logger;
            this.context = context;
            this.bus = bus;
        }

        public async Task Consume(ConsumeContext<RemoveChargeFromOrderItemCommand> consumeContext)
        {
            var message = consumeContext.Message;

            var order = await context.Orders
             .IncludeAll()
             .Where(c => c.OrderNo == message.OrderNo)
             .FirstOrDefaultAsync();

            if (order is null)
            {
                throw new Exception();
            }

            var orderItem = order.Items.FirstOrDefault(i => i.Id == message.OrderItemId);

            if (orderItem is null)
            {
                throw new Exception();
            }

            var Charge = orderItem.Charges.FirstOrDefault(x => x.Id == message.ChargeId);

            if (Charge is null)
            {
                throw new Exception();
            }

            orderItem.Charges.Remove(Charge);

            context.OrderCharges.Remove(Charge);

            order.Update();

            await context.SaveChangesAsync();

            await consumeContext.RespondAsync<RemoveChargeFromOrderItemCommandResponse>(new RemoveChargeFromOrderItemCommandResponse());
        }
    }
}