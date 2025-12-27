using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using Orders.Infrastructure.Persistence;
using Orders.UseCases;
using Orders.Infrastructure;
using Orders.Presentation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOrdersPresentation();

builder.Services.AddOrdersUseCases();

builder.Services.AddOrdersInfrastructure(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<OrdersDbContext>();
    db.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.Run();