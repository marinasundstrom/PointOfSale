using System;

using Catalog.Client;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Sales.Application;
using Sales.Infrastructure;

namespace Sales
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

            services.AddHttpContextAccessor();

            // Register the Swagger services
            services.AddOpenApiDocument(config =>
            {
                config.Title = "Sales API";
                config.Version = "v1";
            });

            services.AddHttpClient(nameof(IItemsClient), (sp, http) =>
            {
                http.BaseAddress = new Uri("https://localhost:8080/catalog/");
            })
            .AddTypedClient<IItemsClient>((http, sp) => new ItemsClient(http));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                app.UseOpenApi();
                app.UseSwaggerUi3(c => c.DocumentTitle = "Sales v1");
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}