using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using GWA.Data;
using GWA.DataLog;
using GWA.Classes;
using GWA.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.Mvc.Razor;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;

namespace GWA
{
    //newcomment
    public class Startup
    {
        private readonly IWebHostEnvironment env;
        public static IServiceProvider ServiceProvider { get; private set; }

        public Startup(IWebHostEnvironment e)
        {
            env = e;
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            #region Localization 
            services.AddLocalization(opts => { opts.ResourcesPath = "Resources"; });

            services.AddMvc()
                .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix, opts => { opts.ResourcesPath = "Resources"; })
                .AddDataAnnotationsLocalization();

            services.Configure<RequestLocalizationOptions>(opts =>
            {
                var supportedCultures = new List<CultureInfo>
                    {
                        new CultureInfo("ru"),
                        new CultureInfo("ro"),
                        new CultureInfo("en")
                    };

                opts.DefaultRequestCulture = new RequestCulture("ru");

                // Formatting numbers, dates, etc.
                opts.SupportedCultures = supportedCultures;

                // UI strings that we have localized.
                opts.SupportedUICultures = supportedCultures;
            });
            #endregion


            services.AddControllersWithViews();

            services.AddCors(options =>
            {
                options.AddPolicy("allow_any_origin",
                builder =>
                {
                    builder.AllowAnyOrigin().AllowAnyHeader();
                });
            });

            services.AddMvc();
            services.AddDbContext<AppDbContext>(ServiceLifetime.Transient);

            // Добавляем компрессию
            services.Configure<GzipCompressionProviderOptions>(options => options.Level = System.IO.Compression.CompressionLevel.Optimal);
            services.AddResponseCompression();

            // добавляем сервис с помощью которого можно получить информацию о HttpContext из любого места через DiJ
            services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<IActionContextAccessor, ActionContextAccessor>();

            services.AddControllersWithViews().AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = null);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory, IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
            app.UseResponseCompression();

            // локализация
            var options = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
            app.UseRequestLocalization(options.Value);

            loggerFactory.AddFile("Logs/webapp-{Date}.txt");

            ServiceProvider = serviceProvider;
            var _logger = loggerFactory.CreateLogger("Startup");

            app.UseCors("allow_any_origin");

            //Автоматическая миграция
            try
            {
                using (var scope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
                {
                    // основной контекст данных
                    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                    if (!dbContext.AllMigrationsApplied())
                    {
                        dbContext.Database.Migrate();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Write("Migration error.. \n" + ex.StackTrace);
                _logger.LogCritical(Utils.GetFullError(ex));
            }

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Shared/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            //app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Session}/{action=Index}/{id?}");
            });
        }
    }
}
