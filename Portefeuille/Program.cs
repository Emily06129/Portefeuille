using Microsoft.EntityFrameworkCore;
using Portefeuille.Data;
using Portefeuille.Services;
using Portefeuille.Models;

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

builder.Services.AddScoped<YahooFinanceService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
//GetService         →  "je cherche, si je trouve pas je retourne null"
//GetRequiredService →  "je cherche, si je trouve pas je crie une erreur claire"
using (var serviceScope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
{
    var context = serviceScope.ServiceProvider.GetRequiredService<PortefeuilleContext>();
    //context.Database.EnsureDeleted();
    context.Database.EnsureCreated();

    if (!context.Actif.Any())
    {
        //  ÉTAPE 1 : Crée un Client de démo 
        // On a besoin d'un client avant de créer un portfolio
        // car Portfolio_client a une FK vers Client
        var client = new Client
        {
            Nom = "Client Demo",
            Email = "demo@portefeuille.com",
            Password = "demo1234",
            Budget = 10000f
        };
        context.Client.Add(client);
        context.SaveChanges(); // ← IMPORTANT : sauvegarde maintenant
                               //   pour générer client.Id

        //  ÉTAPE 2 : Crée un Portfolio lié au client 
        // On a besoin d'un portfolio avant de créer les actifs
        // car Actif a une FK vers Portfolio_client
        var portfolio = new Portfolio_client
        {
            ClientId = client.Id,       // ← utilise l'Id généré
            DateCreation = DateTime.UtcNow,
            RendementPrevu = 0f,
            RisqueVolatilite = 0f,
            ScoreSharp = 0f
        };
        context.Portfolio_client.Add(portfolio);
        context.SaveChanges(); // ← IMPORTANT : sauvegarde maintenant
                               //   pour générer portfolio.Id

        
        var actifs = new List<Actif>
        {
            // 10 ACTIONS 
            new() { Nom = "Apple",         Symbole = "AAPL",    Type = "Action", Secteur = "Technologie",  Portfolio_clientId = portfolio.Id },
            new() { Nom = "Microsoft",     Symbole = "MSFT",    Type = "Action", Secteur = "Technologie",  Portfolio_clientId = portfolio.Id },
            new() { Nom = "Tesla",         Symbole = "TSLA",    Type = "Action", Secteur = "Automobile",   Portfolio_clientId = portfolio.Id },
            new() { Nom = "Amazon",        Symbole = "AMZN",    Type = "Action", Secteur = "E-Commerce",   Portfolio_clientId = portfolio.Id },
            new() { Nom = "Google",        Symbole = "GOOGL",   Type = "Action", Secteur = "Technologie",  Portfolio_clientId = portfolio.Id },
            new() { Nom = "NVIDIA",        Symbole = "NVDA",    Type = "Action", Secteur = "Technologie",  Portfolio_clientId = portfolio.Id },
            new() { Nom = "LVMH",          Symbole = "MC.PA",   Type = "Action", Secteur = "Luxe",         Portfolio_clientId = portfolio.Id },
            new() { Nom = "TotalEnergies", Symbole = "TTE.PA",  Type = "Action", Secteur = "Énergie",      Portfolio_clientId = portfolio.Id },
            new() { Nom = "BNP Paribas",   Symbole = "BNP.PA",  Type = "Action", Secteur = "Finance",      Portfolio_clientId = portfolio.Id },
            new() { Nom = "Airbus",        Symbole = "AIR.PA",  Type = "Action", Secteur = "Aéronautique", Portfolio_clientId = portfolio.Id },

            //  10 CRYPTOS 
            new() { Nom = "Bitcoin",   Symbole = "BTC-USD",  Type = "Crypto", Secteur = "Crypto", Portfolio_clientId = portfolio.Id },
            new() { Nom = "Ethereum",  Symbole = "ETH-USD",  Type = "Crypto", Secteur = "Crypto", Portfolio_clientId = portfolio.Id },
            new() { Nom = "BNB",       Symbole = "BNB-USD",  Type = "Crypto", Secteur = "Crypto", Portfolio_clientId = portfolio.Id },
            new() { Nom = "Solana",    Symbole = "SOL-USD",  Type = "Crypto", Secteur = "Crypto", Portfolio_clientId = portfolio.Id },
            new() { Nom = "XRP",       Symbole = "XRP-USD",  Type = "Crypto", Secteur = "Crypto", Portfolio_clientId = portfolio.Id },
            new() { Nom = "Cardano",   Symbole = "ADA-USD",  Type = "Crypto", Secteur = "Crypto", Portfolio_clientId = portfolio.Id },
            new() { Nom = "Avalanche", Symbole = "AVAX-USD", Type = "Crypto", Secteur = "Crypto", Portfolio_clientId = portfolio.Id },
            new() { Nom = "Dogecoin",  Symbole = "DOGE-USD", Type = "Crypto", Secteur = "Crypto", Portfolio_clientId = portfolio.Id },
            new() { Nom = "Polkadot",  Symbole = "DOT-USD",  Type = "Crypto", Secteur = "Crypto", Portfolio_clientId = portfolio.Id },
            new() { Nom = "Chainlink", Symbole = "LINK-USD", Type = "Crypto", Secteur = "Crypto", Portfolio_clientId = portfolio.Id },
        };

        context.Actif.AddRange(actifs);
        context.SaveChanges();
    }

}
app.Run();
