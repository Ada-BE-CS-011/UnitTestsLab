using Microsoft.EntityFrameworkCore;
using OrdersApi.Bad.Domain.Entities;
using OrdersApi.Bad.Infrastructure.Database;

namespace OrdersApi.Bad;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase("OrdersDatabase"));

        var app = builder.Build();
        await SeedDatabaseAsync(app); // Add await here to properly use async

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseAuthorization();
        app.MapControllers();

        app.Run();
    }

    static async Task SeedDatabaseAsync(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        if (await dbContext.Products.AnyAsync())
            return;

        var products = new List<Product>
        {
            new()
            {
                Name = "Mouse",
                Price = 100,
                Stock = 10
            },
            new()
            {
                Name = "Keyboard",
                Price = 250,
                Stock = 10
            },
            new()
            {
                Name = "Monitor",
                Price = 1200,
                Stock = 10
            }
        };

        await dbContext.Products.AddRangeAsync(products);
        await dbContext.SaveChangesAsync();
    }
}
