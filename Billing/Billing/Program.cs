using System;
using System.Globalization;

using Billing;
using Billing.Application;
using Billing.Infrastructure;
using Billing.Infrastructure.Persistence;

using Catalog.Client;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Add builder.Services to the container.

CultureInfo? culture = new("sv-SE");
CultureInfo.DefaultThreadCurrentCulture = culture;
CultureInfo.DefaultThreadCurrentUICulture = culture;

builder.Services
    .AddInfrastructure(builder.Configuration)
    .AddApplication();

builder.Services
    .AddControllers()
    .AddNewtonsoftJson();

builder.Services.AddHttpContextAccessor();

builder.Services.AddSingleton<ReceiptPrinter>();

// Register the Swagger builder.Services
builder.Services.AddOpenApiDocument(config =>
{
    config.Title = "Billing API";
    config.Version = "v1";
});

// REMOVE
builder.Services.AddHttpClient(nameof(IItemsClient), (sp, http) =>
{
    http.BaseAddress = new Uri("https://localhost:8080/catalog/");
})
.AddTypedClient<IItemsClient>((http, sp) => new ItemsClient(http));

var app = builder.Build();

await app.Services.SeedAsync();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();

    app.UseOpenApi();
    app.UseSwaggerUi3(c => c.DocumentTitle = "Billing v1");
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();