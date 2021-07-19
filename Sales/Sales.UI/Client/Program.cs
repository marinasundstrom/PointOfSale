using System.Globalization;
using System.Net.Http;

using Commerce.Client;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

using MudBlazor.Services;

using Sales.UI.Client;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddMudServices();

CultureInfo? culture = new("sv-SE");
CultureInfo.DefaultThreadCurrentCulture = culture;
CultureInfo.DefaultThreadCurrentUICulture = culture;

builder.Services.AddHttpClient(nameof(IOrdersClient), (sp, http) =>
{
    http.BaseAddress = new Uri($"https://localhost:8080/api/");
})
.AddTypedClient<IOrdersClient>((http, sp) => new OrdersClient(http));

builder.Services.AddHttpClient(nameof(IOrderStatusesClient), (sp, http) =>
{
    http.BaseAddress = new Uri($"https://localhost:8080/api/");
})
.AddTypedClient<IOrderStatusesClient>((http, sp) => new OrderStatusesClient(http));

builder.Services.AddHttpClient(nameof(ICatalogItemsClient), (sp, http) =>
{
    http.BaseAddress = new Uri($"https://localhost:8080/api/");
})
.AddTypedClient<ICatalogItemsClient>((http, sp) => new CatalogItemsClient(http));

await builder.Build().RunAsync();