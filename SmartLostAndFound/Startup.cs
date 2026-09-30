using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SmartLostAndFound.Data;
using SmartLostAndFound.Models;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllersWithViews();
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlite(Configuration.GetConnectionString("DefaultConnection")));
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment environment)
    {
        if (environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseStaticFiles();
        app.UseRouting();
        app.UseAuthorization();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
        });

        using var scope = app.ApplicationServices.CreateScope();
        var database = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        database.Database.EnsureCreated();
        EnsureItemColumns(database);
        if (!database.LostFoundItems.Any())
        {
            database.LostFoundItems.AddRange(
                new LostFoundItem
                {
                    Name = "Blue water bottle",
                    Description = "Reusable bottle found near the library entrance.",
                    Category = "Personal items",
                    IsClaimed = false
                },
                new LostFoundItem
                {
                    Name = "House keys",
                    Description = "Three keys on a red fabric keychain.",
                    Category = "Keys",
                    IsClaimed = false
                },
                new LostFoundItem
                {
                    Name = "Wireless earbuds case",
                    Description = "Black charging case found in the student lounge.",
                    Category = "Electronics",
                    IsClaimed = true
                });
            database.SaveChanges();
        }
    }

    private static void EnsureItemColumns(ApplicationDbContext database)
    {
        var connection = database.Database.GetDbConnection();
        var closeConnection = connection.State == ConnectionState.Closed;
        if (closeConnection)
        {
            connection.Open();
        }

        var columns = new HashSet<string>();
        using (var command = connection.CreateCommand())
        {
            command.CommandText = "PRAGMA table_info('LostFoundItems')";
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                columns.Add(reader.GetString(1));
            }
        }

        if (closeConnection)
        {
            connection.Close();
        }

        var newColumns = new Dictionary<string, string>
        {
            ["ImagePath"] = "ALTER TABLE LostFoundItems ADD COLUMN \"ImagePath\" TEXT NULL",
            ["FinderName"] = "ALTER TABLE LostFoundItems ADD COLUMN \"FinderName\" TEXT NULL",
            ["FinderEmail"] = "ALTER TABLE LostFoundItems ADD COLUMN \"FinderEmail\" TEXT NULL",
            ["PickupLocation"] = "ALTER TABLE LostFoundItems ADD COLUMN \"PickupLocation\" TEXT NULL",
            ["PickupInstructions"] = "ALTER TABLE LostFoundItems ADD COLUMN \"PickupInstructions\" TEXT NULL"
        };
        foreach (var column in newColumns)
        {
            if (!columns.Contains(column.Key))
            {
                database.Database.ExecuteSqlRaw(column.Value);
            }
        }

        database.Database.ExecuteSqlRaw(
            "UPDATE LostFoundItems SET ImagePath = COALESCE(ImagePath, ''), FinderName = COALESCE(FinderName, ''), FinderEmail = COALESCE(FinderEmail, ''), PickupLocation = COALESCE(PickupLocation, ''), PickupInstructions = COALESCE(PickupInstructions, '')");
    }
}