using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore;
using GWA.Data;
using GWA.DataLog;
using GWA.Classes;

namespace GWA
{
    public class Program
    {
        private static IConfiguration config;
        public static void Main(string[] args)
        {
            config = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("hosting.json", optional: true)
                        .AddJsonFile("appsettings.json", optional: false)
                        .Build();

            BuildWebHost(args).Run();
            //CreateHostBuilder(args).Build().Run();
        }
        
        // этот класс нужен для того, чтобы в консоли диспетчера пакетов можно было миграции создавать
        public class DbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
        {
            public AppDbContext CreateDbContext(string[] args)
            {
                var app = BuildWebHostDuringGen(args);
                var scope = app.Services.GetService<IServiceScopeFactory>().CreateScope();

                return scope.ServiceProvider.GetRequiredService<AppDbContext>();
            }
        }

        // этот класс нужен для того, чтобы в консоли диспетчера пакетов можно было миграции создавать
        public class LogDbContextFactory : IDesignTimeDbContextFactory<LogDbContext>
        {
            public LogDbContext CreateDbContext(string[] args)
            {
                var app = BuildWebHostDuringGen(args);
                var scope = app.Services.GetService<IServiceScopeFactory>().CreateScope();

                return scope.ServiceProvider.GetRequiredService<LogDbContext>();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        public static IWebHost BuildWebHostDuringGen(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>().Build();
        }
        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseConfiguration(config)
                .Build();
    }
}
