using System.Threading.Tasks;

using Carts.Infrastructure.Persistence;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Carts
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var app = CreateHostBuilder(args).Build();

            await app.Services.SeedAsync();

            await app.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}