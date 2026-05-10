using OrdersApi.Good.Application;
using OrdersApi.Good.Application.Interfaces;
using OrdersApi.Good.Application.Services;
using OrdersApi.Good.Domain.Interfaces;
using OrdersApi.Good.Infrastructure.Payments;
using OrdersApi.Good.Infrastructure.Repositories;

namespace OrdersApi.Good;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddSwaggerGen();

        builder.Services.AddSingleton<IProductRepository, InMemoryProductRepository>();
        builder.Services.AddSingleton<IOrderRepository, InMemoryOrderRepository>();
        builder.Services.AddSingleton<IPaymentGateway, FakePaymentGateway>();
        builder.Services.AddSingleton<IDateTimeProvider, SystemDateTimeProvider>();
        builder.Services.AddScoped<IOrderService, OrderService>();

        var app = builder.Build();

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
}
