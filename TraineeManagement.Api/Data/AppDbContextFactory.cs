using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace TraineeManagement.Api.Data;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        // 1. Resolve application root directory
        var basePath = Directory.GetCurrentDirectory();
        
        // 2. Load configuration settings profiles
        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        // 3. Extract your connection parameters
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException("Could not find connection string named 'DefaultConnection' inside appsettings.json.");
        }

        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        
        // 4. Bind the database provider to the design-time options builder
        // IF USING ORACLE'S DRIVER (MySql.EntityFrameworkCore):
        optionsBuilder.UseMySQL(connectionString);

        // IF USING POMELO'S DRIVER (Pomelo.EntityFrameworkCore.MySql), UNCOMMENT BELOW instead:
        // optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));

        // 5. Pass the configured options and configuration downstream safely
        return new AppDbContext(optionsBuilder.Options, configuration);
    }
}
