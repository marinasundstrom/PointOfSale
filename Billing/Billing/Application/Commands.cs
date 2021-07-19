using System.Net.Mail;

using Billing.Contracts;
using Billing.Domain.Entities;
using Billing.Infrastructure.Persistence;

using Catalog.Client;

using IronPdf;

using MassTransit;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using OrderPriceCalculator;

using RazorLight;

namespace Billing.Application;

public class CreateReceiptCommandHandler : IConsumer<CreateReceiptCommand>
{
    private readonly ILogger<CreateReceiptCommandHandler> _logger;
    private readonly BillingContext context;
    private readonly IItemsClient catalogItemsClient;
    private readonly IBus bus;

    public CreateReceiptCommandHandler(
        ILogger<CreateReceiptCommandHandler> logger,
        BillingContext context,
        IItemsClient catalogItemsClient,
        IBus bus)
    {
        _logger = logger;
        this.context = context;
        this.catalogItemsClient = catalogItemsClient;
        this.bus = bus;
    }

    public async Task Consume(ConsumeContext<CreateReceiptCommand> consumeContext)
    {
        var dto = consumeContext.Message.Dto;

        var receipt = new Receipt();
        receipt.Id = Guid.NewGuid();
        receipt.Date = DateTime.Now;
        receipt.Type = (Domain.Enums.ReceiptType)dto.ReceiptType;

        try
        {
            receipt.ReceiptNo = await context.Receipts.MaxAsync(r => r.ReceiptNo) + 1;
        }
        catch
        {
            receipt.ReceiptNo = 1;
        }

        foreach (var item in dto.Items)
        {
            await AddItem(receipt, item);
        }

        if (dto.Charges is not null)
        {
            foreach (var item in dto.Charges)
            {
                AddCharge(receipt, item);
            }
        }

        if (dto.Discounts is not null)
        {
            foreach (var item in dto.Discounts)
            {
                AddDiscount(receipt, item);
            }
        }

        receipt.Payment = dto.Payment is not null ? new PaymentDetails
        {
            Paid = dto.Payment.Paid,
            Returned = dto.Payment.Paid > receipt.Total ? (dto.Payment.Paid - receipt.Total) : null
        } : null;

        receipt.Signature = dto.Signature;

        AddCustomFields(receipt, dto);

        context.Receipts.Add(receipt);

        receipt.Update();

        await context.SaveChangesAsync();

        await bus.Publish(new ReceiptCreatedEvent(receipt.ReceiptNo));
        var dto2 = Mappings.CreateReceiptDto(receipt);

        await consumeContext.RespondAsync<ReceiptDto>(dto2);
    }

    private void AddCustomFields(Receipt receipt, CreateReceiptDto dto)
    {
        if (dto.CustomFields is not null)
        {
            foreach (KeyValuePair<string, string> i in dto.CustomFields)
            {
                receipt.CustomFields.Add(new CustomField()
                {
                    Id = Guid.NewGuid(),
                    CustomFieldId = i.Key,
                    Value = i.Value.ToString()
                });
            }
        }
    }

    private void AddCustomFields(ReceiptItem receiptItem, CreateReceiptItemDto dto)
    {
        if (dto.CustomFields is not null)
        {
            foreach (KeyValuePair<string, string> i in dto.CustomFields)
            {
                receiptItem.CustomFields.Add(new CustomField()
                {
                    Id = Guid.NewGuid(),
                    CustomFieldId = i.Key,
                    Value = i.Value.ToString()
                });
            }
        }
    }

    private void AddCharge(Receipt receipt, ReceiptChargeDto chargeDto)
    {
        var charge = new ReceiptCharge()
        {
            Id = Guid.NewGuid(),
            Receipt = receipt,
            Quantity = chargeDto.Quantity,
            Limit = chargeDto.Limit,
            Amount = chargeDto.Amount,
            Percent = chargeDto.Percent,
            Description = chargeDto.Description ?? string.Empty
            //ChargeId = chargeDto.ChargeId
        };

        receipt.Charges.Add(charge);
        context.ReceiptCharges.Add(charge);
    }

    private void AddCharge(ReceiptItem receiptItem, ReceiptChargeDto chargeDto)
    {
        var charge = new ReceiptCharge()
        {
            Id = Guid.NewGuid(),
            Quantity = chargeDto.Quantity,
            Limit = chargeDto.Limit,
            Amount = chargeDto.Amount,
            Percent = chargeDto.Percent,
            Description = chargeDto.Description!
            //ChargeId = chargeDto.ChargeId
        };

        receiptItem.Charges.Add(charge);
    }

    private void AddDiscount(Receipt receipt, ReceiptDiscountDto discountDto)
    {
        var discount = new ReceiptDiscount()
        {
            Id = Guid.NewGuid(),
            Quantity = discountDto.Quantity,
            Limit = discountDto.Limit,
            Amount = discountDto.Amount,
            Percent = discountDto.Percent,
            Description = discountDto.Description!,
            DiscountId = discountDto.DiscountId
        };

        receipt.Discounts.Add(discount);
    }

    private void AddDiscount(ReceiptItem receiptItem, ReceiptDiscountDto discountDto)
    {
        var discount = new ReceiptDiscount()
        {
            Id = Guid.NewGuid(),
            Quantity = discountDto.Quantity,
            Limit = discountDto.Limit,
            Amount = discountDto.Amount,
            Percent = discountDto.Percent,
            Description = discountDto.Description,
            DiscountId = discountDto.DiscountId
        };

        receiptItem.Discounts.Add(discount);
    }

    private async Task AddItem(Receipt receipt, CreateReceiptItemDto dto)
    {
        CatalogItemDto? catalogItem = null;

        if (dto.ItemId is not null)
        {
            catalogItem = await catalogItemsClient.GetItemByItemIdAsync(dto.ItemId);
        }

        var receiptItem = new ReceiptItem()
        {
            Id = Guid.NewGuid(),
            ItemId = dto.ItemId,
            Description = dto.Description ?? catalogItem!.Name,
            Unit = dto.Unit ?? catalogItem!.Unit.Name,
            Quantity = dto.Quantity,
            Price = dto.Price ?? (catalogItem!.VatIncluded ? catalogItem.Price : catalogItem.Price.AddVat(catalogItem.VatRate)),
            VatRate = dto.VatRate ?? catalogItem!.VatRate
        };

        if (dto.Charges is not null)
        {
            foreach (var d in dto.Charges)
            {
                AddCharge(receiptItem, d);
            }
        }

        if (dto.Discounts is not null)
        {
            foreach (var d in dto.Discounts)
            {
                AddDiscount(receiptItem, d);
            }
        }

        receipt.Items.Add(receiptItem);

        AddCustomFields(receiptItem, dto);

        receiptItem.Update();

        receipt.Update();
    }
}

public class SendReceiptByEmailCommandHandler : IConsumer<SendReceiptByEmailCommand>
{
    private readonly ILogger<SendReceiptByEmailCommandHandler> _logger;
    private readonly BillingContext context;
    private readonly IItemsClient catalogItemsClient;
    private readonly IBus bus;
    private readonly ReceiptPrinter receiptPrinter;

    public SendReceiptByEmailCommandHandler(
        ILogger<SendReceiptByEmailCommandHandler> logger,
        BillingContext context,
        IItemsClient catalogItemsClient,
        IBus bus,
        ReceiptPrinter receiptPrinter)
    {
        _logger = logger;
        this.context = context;
        this.catalogItemsClient = catalogItemsClient;
        this.bus = bus;
        this.receiptPrinter = receiptPrinter;
    }

    public async Task Consume(ConsumeContext<SendReceiptByEmailCommand> consumeContext)
    {
        var dto = consumeContext.Message;

        var receipt = await context.Receipts
            .Where(r => r.ReceiptNo == dto.ReceiptNo)
            .Include(c => c.Items)
            .ThenInclude(c => c.Charges)
            .Include(c => c.Items)
            .ThenInclude(c => c.Discounts)
            .Include(c => c.CustomFields)
            .Include(c => c.Items)
            .ThenInclude(c => c.CustomFields)
            .Include(c => c.Totals)
            .Include(c => c.Charges)
            .Include(c => c.Discounts)
            .AsNoTracking()
            .FirstOrDefaultAsync();

        if (receipt is null)
        {
            throw new Exception();
        }

        var stream = await receiptPrinter.PrintAsync(receipt);

        MailModel model = new MailModel
        {
            FirstName = "Arne",
            LastName = "Larsson",
            Email = "arne@test.se"
        };

        var defaultNamespaces = new[]
        {
            "System",
            "System.Linq",
            "System.Collections.Generic",
            "RazorLight.Text",
            "RazorLight",
            "Billing.Application"
        };

        var engine = new RazorLightEngineBuilder()
            // required to have a default RazorLightProject type,
            // but not required to create a template from string.
            .UseEmbeddedResourcesProject(typeof(MailModel))
            .SetOperatingAssembly(typeof(MailModel).Assembly)
            //.AddDefaultNamespaces(defaultNamespaces)
            .UseMemoryCachingProvider()
            .Build();

        string template = await File.ReadAllTextAsync("mail-template.cshtml");

        string result = await engine.CompileRenderStringAsync("mail", template, model);

        SmtpClient smtpClient = new SmtpClient("localhost", 25);

        var message = new MailMessage("test@test.com", model.Email);
        message.Subject = "Testmail";
        message.Body = result;
        message.IsBodyHtml = true;
        message.BodyEncoding = System.Text.Encoding.UTF8;
        message.Attachments.Add(new Attachment(stream, "receipt.pdf"));

        await smtpClient.SendMailAsync(message);

        await consumeContext.RespondAsync<SendReceiptByEmailCommandResponse>(new SendReceiptByEmailCommandResponse());
    }
}

public class MailModel
{
    public string FirstName { get; set; } = null!;

    public string? LastName { get; set; }

    public string Email { get; set; } = null!;
}