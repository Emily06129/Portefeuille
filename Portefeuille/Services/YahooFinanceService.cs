using Microsoft.EntityFrameworkCore;
using NodaTime;
using Portefeuille.Data;
using Portefeuille.Models;
using YahooQuotesApi;

namespace Portefeuille.Services
{
    public class YahooFinanceService
    {
        private readonly PortefeuilleContext _context;
        private readonly ILogger<YahooFinanceService> _logger;

        public YahooFinanceService(PortefeuilleContext context,
                                   ILogger<YahooFinanceService> logger)
        {
            _context = context;
            _logger = logger;
        }

        // MÉTHODE PRINCIPALE
        // Parcourt TOUS les actifs en BDD et récupère
        // l'historique Yahoo pour chacun
        public async Task<List<Donneeboursiere>> FetchAllActifsAsync()
        {
            var actifs = await _context.Actif
                .Where(a => !string.IsNullOrWhiteSpace(a.Symbole))
                .ToListAsync();

            _logger.LogInformation("Récupération historique pour {Count} actif(s)...", actifs.Count);

            var resultats = new List<Donneeboursiere>();
            //La boucle traite chaque actif un par un
            foreach (var actif in actifs)
            {
                var donnees = await FetchHistoricalDataAsync(actif);

                if (donnees != null && donnees.Any())
                    resultats.AddRange(donnees);

                await Task.Delay(500);
            }

            _logger.LogInformation("{Count} données sauvegardées au total.", resultats.Count);
            return resultats;
        }

        // MÉTHODE SECONDAIRE
        // Récupère 3 ans d'historique pour UN seul actif
        // et sauvegarde les nouvelles données en BDD
        private async Task<List<Donneeboursiere>> FetchHistoricalDataAsync(Actif actif)
        {
            try
            {
                //  on calcule la date UNE seule fois dans une variable
             
                var dateDebut = DateTime.UtcNow.AddYears(-3);

                // ÉTAPE 1 : Crée le client Yahoo
                // On réutilise dateDebut pour Year, Month et Day
                var yahoo = new YahooQuotesBuilder()
                    .WithHistoryStartDate(Instant.FromUtc(
                        dateDebut.Year,    //  utilise la variable calculée une fois
                        dateDebut.Month,   //  utilise la variable calculée une fois
                        dateDebut.Day,     //  utilise la variable calculée une fois
                        0, 0))
                    .Build();

                _logger.LogInformation("Yahoo historique : {Symbole}", actif.Symbole);

                // ÉTAPE 2 : Appel Yahoo Finance
                // GetHistoryAsync = méthode v7 pour récupérer l'historique
                // Elle retourne un Result<History> :
                //   → Result = enveloppe qui contient soit une valeur, soit une erreur
                //   → History = l'objet qui contient tous les jours de bourse (Ticks)
                Result<History> result = await yahoo.GetHistoryAsync(actif.Symbole!);

                // Vérifie si Yahoo a retourné une erreur
                if (result.HasError)
                {
                    _logger.LogWarning("Erreur pour {Symbole} : {Erreur}",
                        actif.Symbole, result.Error);
                    return new List<Donneeboursiere>();
                }

                // ÉTAPE 3 : Extrait les données
                // history.Ticks = la liste des jours de bourse
                // Chaque "Tick" = 1 jour = 1 ligne avec Date, Open, Close, Volume...
                History history = result.Value;
                var ticks = history.Ticks;

                if (!ticks.Any())
                {
                    _logger.LogWarning("Aucune donnée reçue pour {Symbole}", actif.Symbole);
                    return new List<Donneeboursiere>();
                }

                // ÉTAPE 4 : Anti-doublons
                // Récupère toutes les dates déjà en BDD pour cet actif
                var datesExistantes = await _context.Donneeboursiere
                    .Where(d => d.ActifId == actif.Id)
                    .Select(d => d.Date.Date)
                    .ToListAsync();

                // ÉTAPE 5 : Gestion du fuseau horaire
                // Yahoo renvoie les dates dans le fuseau de la bourse
                // ex: AAPL → "America/New_York", MC.PA → "Europe/Paris"
                var tz = DateTimeZoneProviders.Tzdb
                             .GetZoneOrNull(history.ExchangeTimezoneName)
                         ?? DateTimeZone.Utc;

                // ÉTAPE 6 : Conversion en Donneeboursiere
                var nouvelles = ticks
                    .Where(t => !datesExistantes.Contains(
                        t.Date.InZone(tz).ToDateTimeUnspecified().Date))
                    .Select(t => new Donneeboursiere
                    {
                        ActifId = actif.Id,
                        Cloture = (float)t.Close,
                        Volume = (float)t.Volume,
                        Date = t.Date.InZone(tz).ToDateTimeUnspecified()
                    })
                    .ToList();

                if (!nouvelles.Any())
                {
                    _logger.LogInformation("{Symbole} : déjà à jour.", actif.Symbole);
                    return new List<Donneeboursiere>();
                }

                // ÉTAPE 7 : Sauvegarde en BDD
                _context.Donneeboursiere.AddRange(nouvelles);
                await _context.SaveChangesAsync();

                _logger.LogInformation("{Symbole} : {Count} jours sauvegardés.",
                    actif.Symbole, nouvelles.Count);

                return nouvelles;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur Yahoo pour '{Symbole}'", actif.Symbole);
                return new List<Donneeboursiere>();
            }
        }
    }
}