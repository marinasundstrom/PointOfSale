using System.IO;
using System.Threading.Tasks;

using Billing.Domain.Entities;

using RazorLight;

namespace Billing;

public class ReceiptPrinter
{
    public ReceiptPrinter()
    {
    }

    public async Task<Stream> PrintAsync(Receipt receipt, ReceiptFormat format = ReceiptFormat.Pdf)
    {
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
            .UseEmbeddedResourcesProject(typeof(Receipt))
            .SetOperatingAssembly(typeof(Receipt).Assembly)
            //.AddDefaultNamespaces(defaultNamespaces)
            .UseMemoryCachingProvider()
            .Build();

        string template = await File.ReadAllTextAsync("receipt-template.cshtml");

        string result = await engine.CompileRenderStringAsync("receipt", template, receipt);

        MemoryStream? ms = null;

        if (format == ReceiptFormat.Html)
        {
            ms = new();
            StreamWriter sw = new(ms, System.Text.Encoding.UTF8);

            await sw.WriteAsync(result);

            await sw.FlushAsync();

            ms.Seek(0, SeekOrigin.Begin);
        }
        else
        {
            // Create a PDF from an existing HTML using C#
            var Renderer = new IronPdf.HtmlToPdf();

            ms = Renderer
                .RenderHtmlAsPdf(result).Stream;
            //.SaveAs("example.pdf");
        }

        return ms!;
    }
}