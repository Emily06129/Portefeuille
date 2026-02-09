using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Portefeuille.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

//avec IA a demandé au prof
builder.Services.AddDbContext<PortefeuilleContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("PortefeuilleContext")));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

using (var serviceScope = app.Services.GetService<IServiceScopeFactory>().CreateScope())
{
    var context = serviceScope.ServiceProvider.GetRequiredService<PortefeuilleContext>();
    context.Database.EnsureDeleted();
    context.Database.EnsureCreated();
}


app.Run();
