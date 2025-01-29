using Infrastructure.Data;
using Infrastructure.Repositories;
using Application.Services;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Domain.Interfaces;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddSingleton<DbConnectionFactory>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ProductService>();

builder.Services.AddSingleton<DbConnectionFactory>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();

builder.Services.AddScoped<OrderService>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();


builder.Services.AddScoped<OrderCommandHandler>();
builder.Services.AddScoped<OrderQueryHandler>();


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Product Order API",
        Version = "v1"
    });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Product Order API v1");
});


app.UseRouting();
app.UseAuthorization();
app.MapControllers();

app.Run();
