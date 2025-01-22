using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;
using GWA.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
//using System.Data.Entity;

namespace GWA.DataDashboard
{
    public class DashboardDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        private readonly IHostingEnvironment env;
        private readonly IConfigurationRoot config;
        public DbSet<Operation> Operations { get; set; }
        public DbSet<OperationType> OperationTypes { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<UserConnection> UserConnection { get; set; }
        public DbSet<MainMenu> MainMenu { get; set; }
        public DashboardDbContext(DbContextOptions<DashboardDbContext> options) : base(options)
        {

        }

        public DashboardDbContext(DbContextOptions<DashboardDbContext> options, IHostingEnvironment e) : base(options)
        {
            env = e;
            config = new ConfigurationBuilder()
                           .SetBasePath(env.ContentRootPath)
                           .AddJsonFile("appsettings.json")
                           .Build();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            #region IdentityFramework
            modelBuilder.Entity<ApplicationUser>(entity =>
            {
                entity.ToTable("Users");
                entity
                    .Ignore(i => i.PhoneNumber)
                    .Ignore(i => i.PhoneNumberConfirmed)
                    .Ignore(i => i.TwoFactorEnabled)
                    .Ignore(i => i.AccessFailedCount)
                    .Ignore(i => i.ConcurrencyStamp);
            });

            modelBuilder.Entity<ApplicationRole>(entity =>
            {
                entity.ToTable("Roles");
            });

            modelBuilder.Entity<IdentityRoleClaim<string>>(entity =>
            {
                entity.ToTable("RoleClaims");
            });

            modelBuilder.Entity<IdentityUserClaim<string>>(entity =>
            {
                entity.ToTable("UserClaims");
            });

            modelBuilder.Entity<IdentityUserRole<string>>(entity =>
            {
                entity.ToTable("UserRoles");
            });

            modelBuilder.Entity<IdentityUserLogin<string>>(entity =>
            {
                entity.ToTable("UserLogins");
            });

            modelBuilder.Entity<IdentityUserToken<string>>(entity =>
            {
                entity.ToTable("UserTokens");
            });
            #endregion

            modelBuilder.Entity<Permission>()
                .HasIndex(t => new { t.RoleId, t.OperationId })
                .IsUnique();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(config.GetConnectionString("DashboardPostgres"));

            base.OnConfiguring(optionsBuilder);
        }

        async public Task EnsureSeedData(IServiceProvider applicationServices)
        {
            UserManager<ApplicationUser> userManager = applicationServices.GetRequiredService<UserManager<ApplicationUser>>();
            RoleManager<ApplicationRole> roleManager = applicationServices.GetRequiredService<RoleManager<ApplicationRole>>();
            ILoggerFactory loggerFactory = applicationServices.GetRequiredService<ILoggerFactory>();

            var logger = loggerFactory.CreateLogger("EnsureSeedData");

            // смотрим, есть ли пользователь Developer
            var userDeveloper = await userManager.FindByEmailAsync("developer@gwa-net.com");
            if (userDeveloper == null)
            {
                var developerId = Guid.NewGuid().ToString().ToLower();
                var createUserResult = await userManager.CreateAsync(new ApplicationUser
                {
                    Id = developerId,
                    EmailConfirmed = true,
                    LockoutEnabled = false,
                    LockoutEnd = DateTime.Now,
                    Email = "developer@gwa-net.com",
                    FirstName = "Admin",
                    LastName = "GWA",
                    UserName = "developer",
                }, "nurababa4");

                if (!createUserResult.Succeeded)
                {
                    logger.LogError(String.Join("\r\n", createUserResult.Errors));
                    return;
                }
                else
                    userDeveloper = await userManager.FindByEmailAsync("developer@gwa-net.com");
            }

            // смотрим есть ли роль Developer
            var developerRole = await roleManager.FindByNameAsync("developer");
            if (developerRole == null)
            {
                var createRoleResult = await roleManager.CreateAsync(new ApplicationRole
                {
                    Name = "developer",
                    Alias = "developer",
                    UserId = userDeveloper.Id
                });
                if (!createRoleResult.Succeeded)
                {
                    logger.LogError(String.Join("\r\n", createRoleResult.Errors));
                    return;
                }
                else developerRole = await roleManager.FindByNameAsync("developer");

            }

            // операции заполняем
            var operType = OperationTypes.FirstOrDefault(s => s.Id == "adcf9d16-285c-496c-b8a9-ddb5d972f1a9");
            if (operType == null)
            {
                OperationTypes.Add(new OperationType { Id = "adcf9d16-285c-496c-b8a9-ddb5d972f1a9", Name = "Authorization" });
                SaveChanges();
                operType = OperationTypes.FirstOrDefault(s => s.Id == "adcf9d16-285c-496c-b8a9-ddb5d972f1a9");
            }

            var operSignIn = Operations.FirstOrDefault(s => s.Id == "1c767063-ad79-48fb-b853-5218eddf0ae8");
            if (operSignIn == null)
            {
                Operations.Add(new Operation
                {
                    Id = "1c767063-ad79-48fb-b853-5218eddf0ae8",
                    EnumName = "Signin",
                    Name = "Вход в систему",
                    OperationTypeId = operType.Id
                });
                SaveChanges();
                operSignIn = Operations.FirstOrDefault(s => s.Id == "1c767063-ad79-48fb-b853-5218eddf0ae8");
            }

            var permissionSignIn = Permissions.FirstOrDefault(s => s.OperationId == operSignIn.Id && s.RoleId == developerRole.Id);
            if (permissionSignIn == null)
            {
                Permissions.Add(new Permission
                {
                    OperationId = operSignIn.Id,
                    RoleId = developerRole.Id,
                    Permitted = true
                });
                SaveChanges();
            }

            if (!(await userManager.IsInRoleAsync(userDeveloper, "developer")))
                await userManager.AddToRoleAsync(userDeveloper, "developer");

            if (MainMenu.Count() == 0)
                CreateMainMenu();
        }
        public void CreateMainMenu()
        {
            // пункт меню Разработчик
            MainMenu developerItem = new MainMenu
            {
                ParentId = 0,
                Text = "Владелец системы",
                TextEn = "Владелец системы",
                TextRo = "Владелец системы",
                OrderId = 100,
                Icon = "icon-diamond",
                IsActive = true
            };

            MainMenu.Add(developerItem);
            SaveChanges();

            MainMenu.Add(new MainMenu            // Клиенты
            {
                ParentId = developerItem.Id,
                Text = "Клиенты системы",
                TextEn = "Клиенты системы",
                TextRo = "Клиенты системы",
                Icon = "glyphicon glyphicon-th-list",
                JsFunction = "ShowView('/customer','customer',2)",
                OrderId = 0,
                IsActive = true
            });
            MainMenu.Add(new MainMenu            // Группы операций (типы операций)
            {
                ParentId = developerItem.Id,
                Text = "Группы операций",
                TextRo = "Группы операций",
                TextEn = "Группы операций",
                Icon = "fa fa-cube",
                JsFunction = "ShowView('/operationtype','operationtype',3)",
                OrderId = 1,
                IsActive = true
            });
            MainMenu.Add(new MainMenu            // Операции
            {
                ParentId = developerItem.Id,
                Text = "Операции",
                TextEn = "Операции",
                TextRo = "Операции",
                Icon = "fa fa-cubes",
                JsFunction = "ShowView('/operation','operation',4)",
                OrderId = 2,
                IsActive = true
            });

            MainMenu.Add(new MainMenu            // Глобальное меню
            {
                ParentId = developerItem.Id,
                Text = "Глобальное меню",
                TextRo = "Глобальное меню",
                TextEn = "Глобальное меню",
                Icon = "fa fa-bars",
                JsFunction = "ShowView('/mainmenu','mainmenu',5)",
                OrderId = 3,
                IsActive = true
            });

            // пункт меню Администрирование
            MainMenu administrationItem = new MainMenu
            {
                ParentId = 0,
                OrderId = 99,
                Text = "Администрирование",
                TextEn = "Администрирование",
                TextRo = "Администрирование",
                Icon = "icon-settings",
                IsActive = true
            };

            MainMenu.Add(administrationItem);
            SaveChanges();

            MainMenu.Add(new MainMenu            // Роли
            {
                ParentId = administrationItem.Id,
                Text = "Роли",
                TextEn = "Роли",
                TextRo = "Роли",
                Icon = "fa fa-user-md",
                JsFunction = "ShowView('/role','role',7)",
                OrderId = 0,
                IsActive = true
            });

            MainMenu.Add(new MainMenu            // Права доступа
            {
                ParentId = administrationItem.Id,
                Text = "Права доступа",
                TextEn = "Права доступа",
                TextRo = "Права доступа",
                Icon = "fa fa-key",
                JsFunction = "ShowView('/permission','permission',8)",
                OrderId = 1,
                IsActive = true
            });

            MainMenu.Add(new MainMenu            // Пользователи
            {
                ParentId = administrationItem.Id,
                Text = "Пользователи",
                TextEn = "Пользователи",
                TextRo = "Пользователи",
                Icon = "icon-user",
                JsFunction = "ShowView('/user','user',9)",
                OrderId = 2,
                IsActive = true
            });
            SaveChanges();
        }
    }
}
