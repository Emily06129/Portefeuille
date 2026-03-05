using Microsoft.EntityFrameworkCore;
using Portefeuille.Data;
using Portefeuille.Models;              
using YahooFinanceApi;                  // La librairie Yahoo Finance


namespace Portefeuille.Services
{
    public class YahooFinanceService
    {
        private PortefeuilleContext _context;
        private ILogger<YahooFinanceService> _logger;

        public YahooFinanceService(PortefeuilleContext context,
                            ILogger<YahooFinanceService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<Donneeboursiere>> FetchHistoricalDataAsync(int actifId)
        {
            //  1. On cherche l'actif dans ta BDD 
            // FindAsync cherche par clé primaire (Id)
            var actif = await _context.Actif.FindAsync(actifId);

            if (actif == null || string.IsNullOrWhiteSpace(actif.Symbole))
            {
                _logger.LogWarning("Actif invalide ou symbole manquant.");
                return null;
            }

            // Si l'actif n'a pas de Symbole renseigné → on arrête
            // Ex: si Symbole est null ou vide "", Yahoo ne sait pas quoi chercher
            if (string.IsNullOrWhiteSpace(actif.Symbole))
            {
                _logger.LogWarning(" Actif '{Nom}' n'a pas de Symbole (ex: AAPL).", actif.Nom);
                return null;
            }

            try
            {
                //  2. On appelle Yahoo Finance 
                // Yahoo.Symbols() = on lui dit quel symbole on veut
                // .Fields()       = on lui dit quels champs on veut
                // .QueryAsync()   = on envoie la requête
                var securities = await Yahoo.Symbols(actif.Symbole)
                    .Fields(
                        Field.RegularMarketPrice,   // Prix actuel (→ Cloture)
                        Field.RegularMarketVolume   // Volume échangé (→ Volume)
                    )
                    .QueryAsync();

                // Si Yahoo ne connaît pas ce symbole → on arrête
                if (!securities.ContainsKey(actif.Symbole))
                {
                    _logger.LogWarning(" Symbole '{Symbole}' inconnu sur Yahoo.", actif.Symbole);
                    return null;
                }

                // On récupère les données de notre symbole
                var data = securities[actif.Symbole];

                //  3. On construit la DonneeBoursiere 
                
                var donnee = new Donneeboursiere
                {
                    ActifId = actifId,                               // Lien vers l'actif
                    Cloture = (float)data[Field.RegularMarketPrice], // Prix actuel
                    Volume = (float)data[Field.RegularMarketVolume],// Volume
                    Date = DateTime.UtcNow                        // Maintenant (heure UTC)
                };

                //  4. On sauvegarde en BDD 
                // Add()             = prépare l'insertion
                // SaveChangesAsync()= exécute le INSERT en SQL
                _context.Donneeboursiere.Add(donnee);
                await _context.SaveChangesAsync();

                // Log de succès visible dans la console Visual Studio
                _logger.LogInformation(
                    " {Symbole} → Clôture: {Prix}$ | Volume: {Vol} | {Date}",
                    actif.Symbole, donnee.Cloture, donnee.Volume, donnee.Date);

                return donnee;
            }
            catch (Exception ex)
            {
                // Si Yahoo est indisponible ou réseau coupé
                _logger.LogError(ex, " Erreur Yahoo pour '{Symbole}'", actif.Symbole);
                return null;
            }
        }

        public async Task<List<Donneeboursiere>> FetchAllActifsAsync()
        {
            // Récupère uniquement les actifs avec un Symbole renseigné
            var actifs = await _context.Actif
                .Where(a => a.Symbole != null && a.Symbole != "")
                .ToListAsync();

            _logger.LogInformation(" Récupération pour {Count} actif(s)...", actifs.Count);

            var resultats = new List<Donneeboursiere>();

            // Pour chaque actif, on appelle la méthode 1
            foreach (var actif in actifs)
            {
                var donnee = await FetchAndSaveAsync(actif.Id);

                if (donnee != null)
                    resultats.Add(donnee);

                //  IMPORTANT : pause de 400ms entre chaque appel
                // Yahoo Finance bloque si on fait trop d'appels trop vite
                await Task.Delay(400);
            }

            _logger.LogInformation(" {Count} actif(s) mis à jour.", resultats.Count);

            return resultats;
        }

    }
}
