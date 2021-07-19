namespace WebApi.Orders;

using Billing.Contracts;

using Sales.Contracts;

using WebApi.Hal;
using WebApi.Hal.Forms;

public partial class Mapper
{
    private readonly UrlBuilder urlBuilder;

    public Mapper(UrlBuilder urlBuilder)
    {
        this.urlBuilder = urlBuilder;
    }

    public WebApi.Orders.Orders Map(IEnumerable<OrderDto> orders, string[] embed)
    {
        var r = new WebApi.Orders.Orders()
        {
            Embedded = new WebApi.Orders.OrdersEmbedded
            {
                Orders = orders.Select(i =>
                    Map(i, embed))
            },
            Links = new Dictionary<string, Link>()
        };

        r.Templates = new Dictionary<string, Template>();

        r.Templates.Add("createOrder", new Template
        {
            Title = "Create Order",
            Method = "POST",
            Target = $"{urlBuilder.GetHostUrl()}orders",
        });

        return r;
    }

    public WebApi.Orders.Order Map(OrderDto order, string[] embed, ReceiptDto? receipt = null)
    {
        var r = new WebApi.Orders.Order()
        {
            Id = order.Id,
            OrderNo = order.OrderNo,
            //Type = (Contracts.OrderType)order.Type,
            Date = order.OrderDate.GetValueOrDefault(),
            Status = order.Status.Id,

            SubTotal = order.SubTotal,

            Vat = order.Vat,
            VatRate = order.VatRate,

            Discount = order.Discount,
            Charge = order.Charge,

            Rounding = order.Rounding,
            Total = order.Total,

            Totals = (order.Totals != null && order.Totals.Any()) ? order.Totals.ToDictionary(x => x.Key.ToString(), x => new OrderTotal()
            {
                SubTotal = x.Value.SubTotal,
                Vat = x.Value.Vat,
                Total = x.Value.Total
            }) : null,

            /*

            Payment = order.Payment is not null ? new Sales.Contracts.PaymentDetailsDto
            {
                Paid = order.Payment.Paid,
                Returned = order.Payment.Returned
            } : null,

            Signature = order.Signature,

            */

            CustomFields = order.CustomFields.Any() ? order.CustomFields.ToDictionary(x => x.Key, x => x.Value) : null,

            Embedded = embed.Length > 0 ? new WebApi.Orders.OrderEmbedded
            {
                Items = embed.Length > 0 && (embed.Contains("all") || embed.Contains("items")) ? order.Items.Select(i =>
                    Map(order.OrderNo, i, embed)) : null,

                Discounts = embed.Length > 0 && (embed.Contains("all") || embed.Contains("discounts")) ? order.Discounts.Select(d =>
                    Map(order.OrderNo, null, d, Array.Empty<string>())) : null,

                Charges = embed.Length > 0 && (embed.Contains("all") || embed.Contains("charges")) ? order.Charges.Select(c =>
                    Map(order.OrderNo, null, c, Array.Empty<string>())) : null,

                CustomFields = embed.Length > 0 && embed.Any(x => x == "customFields") ? order.CustomFields.Select(cf =>
                    Map(order.OrderNo, null, cf, Array.Empty<string>())) : null
            } : null,

            Links = new Dictionary<string, Link> {
                { "self", new Link { Href = $"{urlBuilder.GetHostUrl()}orders/{order.OrderNo}" } },
            }
        };

        if (receipt != null)
        {
            r.Links.Add("receipt", new Link
            {
                Href = $"{urlBuilder.GetHostUrl()}receipts/{receipt.ReceiptNo}",
                Title = "Get the receipt"
            });
        }

        if (r.Templates == null)
        {
            r.Templates = new Dictionary<string, Template>();
        }

        if (true )
        {
            r.Templates.Add("updateStatus", new Template
            {
                Title = "Update status",
                Method = "PUT",
                Properties = new List<Property>() {
                    new Property() {
                        Name = "orderStatusId",
                        Prompt = "Order Status Id",
                        Required = true,
                        Options = new Options()
                        {
                            Link = new FormLink()
                            {
                                Href = $"{urlBuilder.GetHostUrl()}orderStatuses",
                                Templated = false,
                                Type = "application/json"
                            },
                            PromptField = "name",
                            ValueField = "id",
                            MinItems = 1,
                            MaxItems = 1
                        }
                    }
                },
                Target = $"{urlBuilder.GetHostUrl()}orders/{order.OrderNo}/status",
            });
        }

        if (order.Status.Id != "placed")
        {
            if (r.Templates == null)
            {
                r.Templates = new Dictionary<string, Template>();
            }

            r.Templates.Add("place", new Template
            {
                Title = "Place order",
                Method = "POST",
                Target = $"{urlBuilder.GetHostUrl()}orders/{order.OrderNo}/place",
            });
        }

        if (r.Templates == null)
        {
            r.Templates = new Dictionary<string, Template>();
        }

        r.Templates.Add("addItem", new Template
        {
            Title = "Add item",
            Method = "POST",
            Properties = new List<Property>() {
                new Property() {
                    Name = "description",
                    Prompt = "Description",
                    Required = false,
                    Type = PropertyType.Text
                },
                new Property() {
                    Name = "itemId",
                    Prompt = "Item Id",
                    Required = false,
                    Type = PropertyType.Text
                },
                new Property() {
                    Name = "unit",
                    Prompt = "Unit",
                    Required = false,
                    Type = PropertyType.Text
                },
                new Property() {
                    Name = "quantity",
                    Prompt = "Quantity",
                    Required = true,
                    Value = "1",
                    Type = PropertyType.Number
                }
            },
            Target = $"{urlBuilder.GetHostUrl()}orders/{order.OrderNo}/items",
        });

        r.Templates.Add("addCustomField", new Template
        {
            Title = "Add custom field",
            Method = "POST",
            Properties = new List<Property>() {
                new Property() {
                    Name = "name",
                    Prompt = "Name",
                    Required = true,
                    Type = PropertyType.Text
                },
                new Property() {
                    Name = "value",
                    Prompt = "Value",
                    Required = true,
                    Type = PropertyType.Text
                },
            },
            Target = $"{urlBuilder.GetHostUrl()}orders/{order.OrderNo}/customFields",
        });

        return r;
    }

    public WebApi.Orders.OrderItem Map(int orderNo, OrderItemDto i, string[] embed)
    {
        var item = new WebApi.Orders.OrderItem()
        {
            Id = i.Id,
            ItemId = i.ItemId,
            Description = i.Description,
            //Unit = i.Unit,
            Price = i.Price,
            VatRate = i.VatRate,
            Quantity = i.Quantity,
            Charge = i.Charge,
            Discount = i.Discount,
            Total = i.Total,

            CustomFields = i.CustomFields.Any() ? i.CustomFields.ToDictionary(x => x.Key, x => x.Value) : null,

            Embedded = embed.Length > 0 ? new WebApi.Orders.OrderItemEmbedded
            {
                Discounts = embed.Length > 0 && (embed.Contains("all") || embed.Contains("discounts")) ? i.Discounts.Select(d =>
                    Map(orderNo, i.Id, d, Array.Empty<string>())) : null,

                Charges = embed.Length > 0 && (embed.Contains("all") || embed.Contains("charges")) ? i.Charges.Select(c =>
                    Map(orderNo, i.Id, c, Array.Empty<string>())) : null,

                CustomFields = embed.Length > 0 && embed.Any(x => x == "customFields") ? i.CustomFields.Select(cf =>
                    Map(orderNo, i.Id, cf, Array.Empty<string>())) : null
            } : null,

            Links = new Dictionary<string, Link> {
                { "self", new Link { Href = $"{urlBuilder.GetHostUrl()}orders/{orderNo}/items/{i.Id}" } }
            }
        };

        item.Links.Add("order", new Link { Href = $"{urlBuilder.GetHostUrl()}orders/{orderNo}" });

        item.Links.Add("item", new Link { Href = $"{urlBuilder.GetHostUrl()}catalog/items/{i.ItemId}" });

        item.Templates = new Dictionary<string, Template>();

        item.Templates.Add("remove", new Template
        {
            Title = "Remove item",
            Method = "DELETE",
            Target = $"{urlBuilder.GetHostUrl()}orders/{orderNo}/items/{i.Id}",
        });

        if (true /* order.StatusId != "placed" */)
        {
            item.Templates.Add("updateQuantity", new Template
            {
                Title = "Update quantity",
                Method = "PUT",
                Properties = new List<Property>() {
                    new Property() {
                        Name = "quantity",
                        Prompt = "Quantity",
                        Required = true,
                        Type = PropertyType.Number
                    }
                },
                Target = $"{urlBuilder.GetHostUrl()}orders/{orderNo}/items/{i.Id}/quantity",
            });
        }

        item.Templates.Add("addCustomField", new Template
        {
            Title = "Add custom field",
            Method = "POST",
            Properties = new List<Property>() {
                new Property() {
                    Name = "name",
                    Prompt = "Name",
                    Required = true
                },
                new Property() {
                    Name = "value",
                    Prompt = "Value",
                    Required = true
                },
            },
            Target = $"{urlBuilder.GetHostUrl()}orders/{orderNo}items/{i.Id}/customFields",
        });

        return item;
    }

    public WebApi.Orders.OrderDiscount Map(int? orderNo, Guid? orderItemId, OrderDiscountDto i, string[] embed)
    {
        var c = new WebApi.Orders.OrderDiscount()
        {
            Id = i.Id,
            Description = i.Description,
            DiscountId = i.DiscountId,
            Amount = i.Amount,
            Percent = i.Percent,
            Quantity = i.Quantity,
            Total = i.Total
        };

        c.Links.Add("self", new Link
        {
            Href = orderItemId == null
                ? $"{urlBuilder.GetHostUrl()}orders/{orderNo}/discounts/{i.Id}"
                : $"{urlBuilder.GetHostUrl()}orders/{orderNo}/items/{orderItemId}/discounts/{i.Id}"
        });

        c.Templates = new Dictionary<string, Template>();

        c.Templates.Add("delete", new Template
        {
            Title = "Delete discount",
            Method = "DELETE",
            Target = orderItemId == null
                ? $"{urlBuilder.GetHostUrl()}orders/{orderNo}/discounts/{i.Id}"
                : $"{urlBuilder.GetHostUrl()}orders/{orderNo}/items/{orderItemId}/discounts/{i.Id}",
        });

        return c;
    }

    public WebApi.Orders.OrderCharge Map(int? orderNo, Guid? orderItemId, OrderChargeDto i, string[] embed)
    {
        var d = new WebApi.Orders.OrderCharge()
        {
            Id = i.Id,
            Description = i.Description,
            ChargeId = i.ChargeId,
            Amount = i.Amount,
            Percent = i.Percent,
            Quantity = i.Quantity,
            Total = i.Total
        };

        d.Links.Add("self", new Link
        {
            Href = orderNo != null
                ? $"{urlBuilder.GetHostUrl()}orders/{orderNo}/charges/{i.Id}"
                : $"{urlBuilder.GetHostUrl()}orders/{orderNo}/items/{orderItemId}/charges/{i.Id}"
        });

        d.Templates = new Dictionary<string, Template>();

        d.Templates.Add("delete", new Template
        {
            Title = "Delete charge",
            Method = "DELETE",
            Target = orderNo != null
                ? $"{urlBuilder.GetHostUrl()}orders/{orderNo}/charges/{i.Id}"
                : $"{urlBuilder.GetHostUrl()}orders/{orderNo}/items/{orderItemId}/charges/{i.Id}",
        });

        return d;
    }

    public WebApi.Shared.CustomField Map(int? orderNo, Guid? orderItemId, KeyValuePair<string, object> i, string[] embed)
    {
        var d = new WebApi.Shared.CustomField()
        {
            Id = i.Key,
            Name = i.Key,
            Type = null!,
            Value = i.Value
        };

        d.Links.Add("self", new Link
        {
            Href = orderItemId == null
                ? $"{urlBuilder.GetHostUrl()}orders/{orderNo}/customFields/{i.Key}"
                : $"{urlBuilder.GetHostUrl()}orders/{orderNo}/items/{orderItemId}/customFields/{i.Key}"
        });

        if (d.Templates == null)
        {
            d.Templates = new Dictionary<string, Template>();
        }

        d.Templates.Add("update", new Template
        {
            Title = "Update custom field",
            Method = "PUT",
            Properties = new List<Property>()
            {
                new Property() {
                    Name = "value",
                    Prompt = "Value",
                    Required = true
                }
            },
            Target = orderItemId == null
                ? $"{urlBuilder.GetHostUrl()}orders/{orderNo}/customFields/{i.Key}"
                : $"{urlBuilder.GetHostUrl()}orders/{orderNo}/items/{orderItemId}/customFields/{i.Key}"
        });

        d.Templates.Add("delete", new Template
        {
            Title = "Delete custom field",
            Method = "DELETE",
            Target = orderItemId == null
                ? $"{urlBuilder.GetHostUrl()}orders/{orderNo}/customFields/{i.Key}"
                : $"{urlBuilder.GetHostUrl()}orders/{orderNo}/items/{orderItemId}/customFields/{i.Key}"
        });

        return d;
    }
}