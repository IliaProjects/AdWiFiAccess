using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using GWA.DataLog.Models;
using Microsoft.Extensions.Hosting;
using System;

namespace GWA.DataLog
{
    public class LogDbContext : DbContext
    {
        private readonly IHostingEnvironment env;
        private readonly IConfigurationRoot config;
        public LogDbContext(DbContextOptions<LogDbContext> options) : base(options)
        {
            //optionsBuilder.UseNpgsql(config.GetConnectionString("LogsPostgres"));
        }
        public DbSet<Log> Logs { get; set; }

        public LogDbContext(DbContextOptions<LogDbContext> options, IHostingEnvironment e) : base(options)
        {
            env = e;
            config = new ConfigurationBuilder()
                           .SetBasePath(env.ContentRootPath)
                           .AddJsonFile("appsettings.json")
                           .Build();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Log>(log =>
            {
                log.HasKey(x => x.Id);
                log.Property(x => x.Id).ValueGeneratedOnAdd();
            });

            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(config.GetConnectionString("LogsPostgres"));

            base.OnConfiguring(optionsBuilder);
        }
    }
}
