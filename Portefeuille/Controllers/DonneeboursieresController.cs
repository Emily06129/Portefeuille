using Elfie.Serialization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Portefeuille.Data;
using Portefeuille.Models;
using Portefeuille.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Portefeuille.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DonneeboursieresController : ControllerBase
    {
        private readonly PortefeuilleContext _context;
        private readonly YahooFinanceService _yahooService;
        public DonneeboursieresController(PortefeuilleContext context,
                                          YahooFinanceService yahooService)
        {
            _context = context;
            _yahooService = yahooService;
        }

        // GET: api/Donneeboursieres
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Donneeboursiere>>> GetDonneeboursiere()
        {
            return await _context.Donneeboursiere.ToListAsync();
        }

        // GET: api/Donneeboursieres/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Donneeboursiere>> GetDonneeboursiere(int id)
        {
            var donneeboursiere = await _context.Donneeboursiere.FindAsync(id);

            if (donneeboursiere == null)
            {
                return NotFound();
            }

            return donneeboursiere;
        }

        // PUT: api/Donneeboursieres/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDonneeboursiere(int id, Donneeboursiere donneeboursiere)
        {
            if (id != donneeboursiere.Id)
            {
                return BadRequest();
            }

            _context.Entry(donneeboursiere).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DonneeboursiereExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Donneeboursieres
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Donneeboursiere>> PostDonneeboursiere(Donneeboursiere donneeboursiere)
        {
            _context.Donneeboursiere.Add(donneeboursiere);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDonneeboursiere", new { id = donneeboursiere.Id }, donneeboursiere);
        }

        // DELETE: api/Donneeboursieres/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDonneeboursiere(int id)
        {
            var donneeboursiere = await _context.Donneeboursiere.FindAsync(id);
            if (donneeboursiere == null)
            {
                return NotFound();
            }

            _context.Donneeboursiere.Remove(donneeboursiere);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool DonneeboursiereExists(int id)
        {
            return _context.Donneeboursiere.Any(e => e.Id == id);
        }
    
        // POST: api/Donneeboursieres/fetch/{actifId}
        [HttpPost("fetch/{actifId}")]
        public async Task<IActionResult> FetchForActif(int actifId)
        {
            // Vérifie que l'actif existe
            var actif = await _context.Actif.FindAsync(actifId);
            if (actif == null)
                return NotFound(new { message = $"Actif Id={actifId} introuvable." });

            // Appelle le service — il va chercher l'historique 3 ans pour tous
            // les actifs mais on retourne juste le compte pour cet actif
            var resultats = await _yahooService.FetchAllActifsAsync();
            var count = resultats.Count(d => d.ActifId == actifId);

            return Ok(new
            {
                message = $"Historique récupéré pour {actif.Nom}.",
                donneesAjoutees = count
            });
        }

        // POST: api/Donneeboursieres/fetch-all
        [HttpPost("fetch-all")]
        public async Task<IActionResult> FetchAll()
        {
            var resultats = await _yahooService.FetchAllActifsAsync();

            return Ok(new
            {
                message = $"{resultats.Count} actif(s) mis à jour avec succès.",
                donneesAjoutees = resultats
            });
        }

        // GET: api/Donneeboursieres/actif/
        [HttpGet("actif/{actifId}")]
        public async Task<IActionResult> GetByActif(int actifId, [FromQuery] int limit = 100)
        {
            var donnees = await _context.Donneeboursiere
                .Where(d => d.ActifId == actifId)
                .OrderByDescending(d => d.Date)
                .Take(limit)
                .ToListAsync();

            return Ok(donnees);
        }


        [HttpPost("~/api/prediction/tous")]
        public async Task<IActionResult> PredireTousActifs()
        {
            // Instancie le service de prédiction ML.NET

            var predictionService = new PredictionService();

            // Dictionnaire pour stocker les résultats : Symbole - prix prédits

            var resultats = new Dictionary<string, float[]>();

            // Récupère tous les actifs depuis la base de données

            var actifs = await _context.Actif.ToListAsync();

            // Récupère l'historique des prix de chaque actif, trié par date croissante
            //La boucle foreach parcourt chaque actif, prédit ses 30 prochains prix via ML.NET et les stocke en base — une ligne par jour prédit, liée à son actif via ActifId.
            foreach (var actif in actifs)
            {
                // récupération des données historiques pour chaque actif
                var historique = await _context.Donneeboursiere
                    .Where(d => d.ActifId == actif.Id)
                    .OrderBy(d => d.Date)
                    .ToListAsync();

                if (historique.Count < 30)
                    continue;

                // Prédit les prix pour les 30 prochains jours via ML.NET SSA

                var predictions = predictionService.ForecastPrices(historique, nbJours: 30);

                // Stocke les prédictions avec le symbole de l'actif comme clé ( ce qu'on voit sur postman lorsqu'on post)

                resultats[actif.Symbole] = predictions;

                //  Sauvegarde des info sur les prédictions dans la base de données
                for (int i = 0; i < predictions.Length; i++)
                {
                    _context.DonneesPredites.Add(new DonneesPredites
                    {
                        ActifId = actif.Id,
                        Symbole = actif.Symbole,
                        DatePrediction = DateTime.Now.AddDays(i + 1),
                        PrixPredit = predictions[i]
                    });
                }
            }

            await _context.SaveChangesAsync();

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
            return Ok(resultats);


        }


        // GET: api/DonneesPredites/{symbole} Récupération pour faire le graphe
        [HttpGet("~/api/DonneesPredites/{symbole}")]
        public async Task<IActionResult> GetPredictions(string symbole)
        {
            var donnees = await _context.DonneesPredites
                .Where(d => d.Symbole == symbole)
                .OrderBy(d => d.DatePrediction)
                .ToListAsync();

            return Ok(donnees);
        }

        // POST: api/Donneeboursieres/optimiser
        [HttpPost("optimiser")]
        public async Task<IActionResult> Optimiser([FromBody] List<string> symboles)
        {
            if (symboles == null || symboles.Count < 2)
                return BadRequest("Minimum 2 actifs requis.");

            // 1. Récupérer les actifs depuis la BDD
            var actifs = await _context.Actif
                .Where(a => symboles.Contains(a.Symbole))
                .ToListAsync();

            if (actifs.Count < 2)
                return BadRequest("Actifs introuvables en base.");

            // 2. Récupérer l'historique de chaque actif
            var historiques = new Dictionary<string, List<Donneeboursiere>>();
            foreach (var actif in actifs)
            {
                var hist = await _context.Donneeboursiere
                    .Where(d => d.ActifId == actif.Id)
                    .OrderBy(d => d.Date)
                    .ToListAsync();

                if (hist.Count < 30)
                    return BadRequest($"Historique insuffisant pour {actif.Symbole}.");

                historiques[actif.Symbole] = hist;
            }

            // 3. Prédictions ML.NET pour chaque actif
            var predictionService = new PredictionService();
            var predictions = new Dictionary<string, float[]>();
            foreach (var actif in actifs)
                predictions[actif.Symbole] = predictionService
                    .ForecastPrices(historiques[actif.Symbole], nbJours: 30);

            // 4. Markowitz → 5 portefeuilles optimaux
            var optimiseur = new OptimiseurPortfolio();
            var portefeuilles = optimiseur.GenererPortefeuilles(predictions, historiques);

            return Ok(portefeuilles);
        }
    }



}
