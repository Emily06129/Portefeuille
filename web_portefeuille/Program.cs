using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using web_portefeuille.Data;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<web_portefeuilleContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("web_portefeuilleContext") ?? throw new InvalidOperationException("Connection string 'web_portefeuilleContext' not found.")));

builder.Services.AddHttpClient();
builder.Services.AddSession();
//app.UseSession();

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseSession();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
