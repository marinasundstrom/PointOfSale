using Microsoft.AspNetCore.Http;

using WebApi;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

// Add services to the container.

services
    .AddControllers()
    .AddNewtonsoftJson();

services.AddCors(o => o.AddPolicy("MyPolicy", builder =>
{
    builder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
}));

services.AddHttpContextAccessor();

services.AddScoped<UrlBuilder>();

services.AddApplication();

// Register the Swagger services
services.AddOpenApiDocument(config =>
{
    config.Title = "Commerce API";
    config.Version = "v1";
});

var app = builder.Build();

// Configure the HTTP request pipeline.

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();

    app.UseCors("MyPolicy");

    app.UseOpenApi();
    app.UseSwaggerUi3(c => c.DocumentTitle = "Commerce v1");
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();