using System.Globalization;

using Customers.Application;
using Customers.Infrastructure;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Customers
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

            CultureInfo? culture = new("sv-SE");
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;

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
                config.Title = "Customers API";
                config.Version = "v1";
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                app.UseOpenApi();
                app.UseSwaggerUi3(c => c.DocumentTitle = "Customers v1");
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