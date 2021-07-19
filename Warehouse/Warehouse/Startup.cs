
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Warehouse.Application;
using Warehouse.Infrastructure;

namespace Warehouse
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
                config.Title = "Warehouse API";
                config.Version = "v1";
            });

            services.AddStackExchangeRedisCache(o =>
            {
                o.Configuration = Configuration.GetConnectionString("redis");
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                app.UseOpenApi();
                app.UseSwaggerUi3(c => c.DocumentTitle = "Warehouse v1");
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