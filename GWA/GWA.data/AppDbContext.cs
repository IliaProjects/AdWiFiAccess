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

namespace GWA.Data
{
    public class AppDbContext : DbContext// IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        private readonly IHostingEnvironment env;
        private readonly IConfigurationRoot config;

        public DbSet<Router> Routers { get; set; }
        public DbSet<Bus> Buses { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderShare> OrdersShare { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<SessionHover> SessionsHover { get; set; }
        public DbSet<SessionHoverInvalid> SessionsHoverInvalid { get; set; }
        public DbSet<SessionArchieved> SessionsArchieved { get; set; }
        public DbSet<SessionHoverArchieved> SessionsHoverArchieved { get; set; }
        public DbSet<BindingRouterBus> BindingRouterBuses { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public AppDbContext(DbContextOptions<AppDbContext> options, IHostingEnvironment e) : base(options)
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

            modelBuilder.Entity<Bus>()
                .HasIndex(t => new { t.Nr })
                .IsUnique();

            modelBuilder.Entity<Router>()
                .HasIndex(t => new { t.Nr })
                .IsUnique();

            modelBuilder.Entity<BindingRouterBus>()
                .HasIndex(t => new { t.RouterId })
                .IsUnique();

            modelBuilder.Entity<BindingRouterBus>()
                .HasIndex(t => new { t.BusId })
                .IsUnique();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseNpgsql(config.GetConnectionString("MainPostgres"));

            base.OnConfiguring(optionsBuilder);
        }
    }
}
