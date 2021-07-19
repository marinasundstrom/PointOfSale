namespace WebApi.Carts;

using global::Carts.Contracts;

using WebApi.Hal;
using WebApi.Hal.Forms;

public partial class Mapper
{
    private readonly UrlBuilder urlBuilder;

    public Mapper(UrlBuilder urlBuilder)
    {
        this.urlBuilder = urlBuilder;
    }

    public WebApi.Carts.Carts Map(IEnumerable<CartDto> carts, string[] embed)
    {
        var r = new WebApi.Carts.Carts()
        {
            Embedded = new WebApi.Carts.CartsEmbedded
            {
                Carts = carts.Select(i =>
                    Map(i, embed))
            },
            Links = new Dictionary<string, Link>()
        };

        r.Templates = new Dictionary<string, Template> {
            {
                "default", new Template {
                    Title = "Create",
                    Method = "POST",
                    Properties = new List<Property> {
                        new Property {
                            Name = "tag",
                            Prompt = "Tag",
                            Placeholder = "Tag",
                            Type = PropertyType.Text
                        }
                    },
                    Target = $"{urlBuilder.GetHostUrl()}carts"
                }
            }
        };

        return r;
    }

    public WebApi.Carts.Cart Map(CartDto cart, string[] embed)
    {
        var r = new WebApi.Carts.Cart()
        {
            Id = cart.Id,
            Tag = cart.Tag,

            SubTotal = cart.SubTotal,

            Vat = cart.Vat,
            VatRate = cart.VatRate,

            Discount = cart.Discount,
            Charge = cart.Charge,

            Rounding = cart.Rounding,
            Total = cart.Total,

            Totals = cart.Totals != null ? cart.Totals.ToDictionary(x => x.Key.ToString(), x => new CartTotal()
            {
                SubTotal = x.Value.SubTotal,
                Vat = x.Value.Vat,
                Total = x.Value.Total
            }) : null,

            /*

            Payment = cart.Payment is not null ? new Sales.Contracts.PaymentDetailsDto
            {
                Paid = cart.Payment.Paid,
                Returned = cart.Payment.Returned
            } : null,

            Signature = cart.Signature,

            */

            //CustomFields = cart.CustomFields.Any() ? cart.CustomFields.ToDictionary(x => x.Key, x => x.Value) : null,

            Embedded = embed.Length > 0 ? new WebApi.Carts.CartEmbedded
            {
                Items = embed.Length > 0 && (embed.Contains("all") || embed.Contains("items")) ? cart.Items.Select(i =>
                    Map(cart.Id, i, embed)) : null,
                Discounts = embed.Length > 0 && (embed.Contains("all") || embed.Contains("discounts")) ? cart.Discounts.Select(i =>
                    Map(cart.Id, null, i, Array.Empty<string>())) : null,
                Charges = embed.Length > 0 && (embed.Contains("all") || embed.Contains("charges")) ? cart.Charges.Select(i =>
                    Map(cart.Id, null, i, Array.Empty<string>())) : null
            } : null,
            Links = new Dictionary<string, Link> {
                { "self", new Link { Href = $"{urlBuilder.GetHostUrl()}carts/{cart.Id}" } },
            }
        };

        r.Templates = new Dictionary<string, Template>();

        r.Templates.Add("delete", new Template
        {
            Title = "Delete cart",
            Method = "DELETE",
            Target = $"{urlBuilder.GetHostUrl()}carts/{cart.Id}"
        });

        r.Templates.Add("addItem", new Template
        {
            Title = "Add item to cart",
            Method = "POST",
            Target = $"{urlBuilder.GetHostUrl()}carts/{cart.Id}/items",
            Properties = new List<Property>() {
                new Property() {
                    Name = "description",
                    Prompt = "Description",
                    Required = false,
                    Type = PropertyType.Text
                },
                new Property() {
                    Name = "itemId",
                    Prompt = "ItemId",
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
            }
        });

        r.Templates.Add("clear", new Template
        {
            Title = "Clear all items",
            Method = "POST",
            Target = $"{urlBuilder.GetHostUrl()}carts/{cart.Id}/clear"
        });

        return r;
    }

    public WebApi.Carts.CartItem Map(Guid id, CartItemDto i, string[] embed)
    {
        var item = new WebApi.Carts.CartItem()
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

            //CustomFields = i.CustomFields.Any() ? i.CustomFields.ToDictionary(x => x.Key, x => x.Value) : null,

            Embedded = embed.Length > 0 ? new WebApi.Carts.CartItemEmbedded
            {
                Discounts = embed.Length > 0 && (embed.Contains("all") || embed.Contains("discounts")) ? i.Discounts.Select(i =>
                    Map(id, i.Id, i, Array.Empty<string>())) : null,
                Charges = embed.Length > 0 && (embed.Contains("all") || embed.Contains("charges")) ? i.Charges.Select(i =>
                    Map(id, i.Id, i, Array.Empty<string>())) : null //,
                /* CustomFields = embed.Length > 0 && embed.Any(x => x == "customFields") ? i.CustomFields.Select(i =>
                    Map(id, i.Id, i, Array.Empty<string>())) : null */
            } : null,
            Links = new Dictionary<string, Link> {
                { "self", new Link { Href = $"{urlBuilder.GetHostUrl()}carts/{id}/items/{i.Id}" } }
            }
        };

        item.Links.Add("item", new Link { Href = $"{urlBuilder.GetHostUrl()}catalog/items/{i.ItemId}" });

        item.Templates = new Dictionary<string, Template> {
            {
                "updateQuantity", new Template {
                    Title = "Update quantity",
                    Method = "PUT",
                    Target = $"{urlBuilder.GetHostUrl()}carts/{id}/items/{i.Id}/quantity"
                }
            },
            {
                "remove", new Template {
                    Title = "Remove item",
                    Method = "DELETE",
                    Target = $"{urlBuilder.GetHostUrl()}carts/{id}/items/{i.Id}"
                }
            }
        };

        return item;
    }

    public WebApi.Carts.CartDiscount Map(Guid? id, Guid? cartItemId, CartDiscountDto i, string[] embed)
    {
        var c = new WebApi.Carts.CartDiscount()
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
            Href = cartItemId == null
                ? $"{urlBuilder.GetHostUrl()}carts/{id}/discounts/{i.Id}"
                : $"{urlBuilder.GetHostUrl()}carts/{id}/items/{cartItemId}/discounts/{i.Id}"
        });

        return c;
    }

    public WebApi.Carts.CartCharge Map(Guid? id, Guid? cartItemId, CartChargeDto i, string[] embed)
    {
        var d = new WebApi.Carts.CartCharge()
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
            Href = cartItemId == null
                ? $"{urlBuilder.GetHostUrl()}carts/{id}/charges/{i.Id}"
                : $"{urlBuilder.GetHostUrl()}carts/{id}/items/{cartItemId}/charges/{i.Id}"
        });

        return d;
    }
}