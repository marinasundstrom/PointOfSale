using System;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;

using Billing.Client;

using Blazored.LocalStorage;
using Blazored.Modal;

using Carts.Client;

using Catalog.Client;

using Checkout.Client;

using Customers.Client;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Frontend.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            builder.Services.AddLocalization();

            CultureInfo? culture = new("sv-SE");
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;

            builder.Services.AddBlazoredModal();

            builder.Services.AddBlazoredLocalStorage();

            builder.Services.AddHttpClient(nameof(IItemsClient), (sp, http) =>
            {
                var navigationManager = sp.GetRequiredService<NavigationManager>();
                http.BaseAddress = new Uri($"{navigationManager.BaseUri}catalog/");
            })
            .AddTypedClient<IItemsClient>((http, sp) => new ItemsClient(http));

            builder.Services.AddHttpClient(nameof(ICartClient), (sp, http) =>
            {
                var navigationManager = sp.GetRequiredService<NavigationManager>();
                http.BaseAddress = new Uri($"{navigationManager.BaseUri}cart/");
            })
            .AddTypedClient<ICartClient>((http, sp) => new CartClient(http));

            builder.Services.AddHttpClient(nameof(IReceiptsClient), (sp, http) =>
            {
                var navigationManager = sp.GetRequiredService<NavigationManager>();
                http.BaseAddress = new Uri($"{navigationManager.BaseUri}billing/");
            })
            .AddTypedClient<IReceiptsClient>((http, sp) => new ReceiptsClient(http));

            builder.Services.AddHttpClient(nameof(ICheckoutClient), (sp, http) =>
            {
                var navigationManager = sp.GetRequiredService<NavigationManager>();
                http.BaseAddress = new Uri($"{navigationManager.BaseUri}checkout/");
            })
            .AddTypedClient<ICheckoutClient>((http, sp) => new CheckoutClient(http));

            builder.Services.AddHttpClient(nameof(IPersonsClient), (sp, http) =>
            {
                var navigationManager = sp.GetRequiredService<NavigationManager>();
                http.BaseAddress = new Uri($"{navigationManager.BaseUri}customers/");
            })
            .AddTypedClient<IPersonsClient>((http, sp) => new PersonsClient(http));

            await builder.Build().RunAsync();
        }
    }
}