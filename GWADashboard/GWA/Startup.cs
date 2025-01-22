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
using GWA.DataDashboard;
//using GWA.DataLog;
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


            // Добавляем поддержку сессий
            services.AddSession();

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
            services.AddDbContext<DashboardDbContext>(ServiceLifetime.Transient);

            // подключаем Identity Framework
            services.AddIdentity<ApplicationUser, ApplicationRole>(s =>
            {
                s.SignIn.RequireConfirmedEmail = true;
                s.Password.RequireDigit = false;
                s.Password.RequiredLength = 4;
                s.Password.RequireNonAlphanumeric = false;
                s.Password.RequireUppercase = false;
                s.Password.RequireLowercase = false;
            })
                .AddEntityFrameworkStores<DashboardDbContext>()
                .AddDefaultTokenProviders();

            // добавляем нашу фабрику ClaimsPrincipal
            services.AddScoped<IUserClaimsPrincipalFactory<ApplicationUser>, AppClaimsPrincipalFactory>();

            // Добавляем компрессию
            services.Configure<GzipCompressionProviderOptions>(options => options.Level = System.IO.Compression.CompressionLevel.Optimal);
            services.AddResponseCompression();

            services.ConfigureApplicationCookie(options =>
            {
                // Есть SecurityStamp в котором есть периодическая проверка Claims и если время вышло (например, когда ничего не делаешь или когда сервер остановлен), то происходит
                // вызов AppClaimsPrincipalFactory CreateAsync, что нам не нужно т.к. сессия еще не создана и в ошибку вылетало
                // поэтому стандартную проверку отключаем
                options.Events = new Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationEvents();

                //Cookie settings, only this changes expiration
                //options.Cookie.HttpOnly = true;
                //options.Cookie.Expiration = TimeSpan.FromDays(365 * 10);
                //options.ExpireTimeSpan = TimeSpan.FromDays(365 * 10);
            });

            services.AddAuthentication("CookieAuth").AddCookie("CookieAuth", config =>
            {
                config.Cookie.Name = "GWA.Cookie";
            });

            // добавляем сервис с помощью которого можно получить информацию о HttpContext из любого места через DiJ
            services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<IActionContextAccessor, ActionContextAccessor>();

            services.AddControllersWithViews().AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = null);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostApplicationLifetime applicationLifetime, IWebHostEnvironment env, ILoggerFactory loggerFactory, IServiceProvider serviceProvider)
        {
            loggerFactory.AddFile("Logs/webapp-{Date}.txt");

            ServiceProvider = serviceProvider;
            var _logger = loggerFactory.CreateLogger("Startup");

            app.UseResponseCompression();

            // локализация
            var options = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
            app.UseRequestLocalization(options.Value);

            app.UseCors("allow_any_origin");

            //Автоматическая миграция
            try
            {
                using (var scope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
                {
                    // контекст роутеров
                    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                    if (!dbContext.AllMigrationsApplied())
                    {
                        dbContext.Database.Migrate();
                    }

                    // контекст сайта
                    var dbContextDashboard = scope.ServiceProvider.GetRequiredService<DashboardDbContext>();

                    if (!dbContextDashboard.AllMigrationsApplied())
                    {
                        dbContextDashboard.Database.Migrate();
                        dbContextDashboard.EnsureSeedData(scope.ServiceProvider).Wait();
                    }
                }
            }
            catch (Exception ex)
            {
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

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSession();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
            Console.WriteLine("GWADashboard");//System.Diagnostics.Process.GetCurrentProcess().ProcessName);

            // регистрируем обработчик события OnApplicationStarted
            applicationLifetime.ApplicationStarted.Register(OnApplicationStarted);
            applicationLifetime.ApplicationStopping.Register(OnApplicationStopping);
        }

        protected void OnApplicationStopping()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var _db = scope.ServiceProvider.GetService<AppDbContext>();
                var x = 5;
            }
        }

        protected void OnApplicationStarted()
        {

        }
    }
}
