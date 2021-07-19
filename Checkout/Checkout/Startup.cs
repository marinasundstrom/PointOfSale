using System;
using System.Linq;

using Billing.Client;

using Carts.Client;

using Catalog.Client;

using Checkout.Application;
using Checkout.Infrastructure;

using Customers.Client;

using Marketing.Client;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Payments.Client;

using Sales.Client;

namespace Checkout
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddInfrastructure(Configuration)
                .AddApplication();

            services
                .AddControllers()
                .AddNewtonsoftJson();

            services.AddSignalR();

            services.AddResponseCompression(opts =>
            {
                opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
                    new[] { "application/octet-stream" });
            });

            services.AddHttpContextAccessor();

            // Register the Swagger services
            services.AddOpenApiDocument(config =>
            {
                config.Title = "Checkout API";
                config.Version = "v1";
            });

            services.AddHttpClient(nameof(IItemsClient), (sp, http) =>
            {
                http.BaseAddress = new Uri("https://localhost:8080/catalog/");
            })
            .AddTypedClient<IItemsClient>((http, sp) => new ItemsClient(http));

            services.AddHttpClient(nameof(ICartClient), (sp, http) =>
            {
                http.BaseAddress = new Uri("https://localhost:8080/cart/");
            })
            .AddTypedClient<ICartClient>((http, sp) => new CartClient(http));

            services.AddHttpClient(nameof(IOrdersClient), (sp, http) =>
            {
                http.BaseAddress = new Uri("https://localhost:8080/sales/");
            })
            .AddTypedClient<IOrdersClient>((http, sp) => new OrdersClient(http));

            services.AddHttpClient(nameof(IReceiptsClient), (sp, http) =>
            {
                http.BaseAddress = new Uri("https://localhost:8080/billing/");
            })
            .AddTypedClient<IReceiptsClient>((http, sp) => new ReceiptsClient(http));

            services.AddHttpClient(nameof(IPersonsClient), (sp, http) =>
            {
                http.BaseAddress = new Uri("https://localhost:8080/customers/");
            })
            .AddTypedClient<IPersonsClient>((http, sp) => new PersonsClient(http));

            services.AddHttpClient(nameof(IDiscountsClient), (sp, http) =>
            {
                http.BaseAddress = new Uri("https://localhost:8080/marketing/");
            })
            .AddTypedClient<IDiscountsClient>((http, sp) => new DiscountsClient(http));

            services.AddHttpClient(nameof(IPaymentsClient), (sp, http) =>
            {
                http.BaseAddress = new Uri("https://localhost:8080/payments/");
            })
            .AddTypedClient<IPaymentsClient>((http, sp) => new PaymentsClient(http));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseResponseCompression();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                app.UseOpenApi();
                app.UseSwaggerUi3(c => c.DocumentTitle = "Checkout v1");
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<Hubs.CartHub>("/hubs/cart");
                endpoints.MapHub<Hubs.CatalogHub>("/hubs/catalog");
                endpoints.MapHub<Hubs.PaymentHub>("/hubs/payment");
            });
        }
    }
}