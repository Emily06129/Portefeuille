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

        // GET: api/Donneeboursieres/actif/1
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


[HttpGet("predict/{actifId}")]
        public async Task<IActionResult> PredictPrices(int actifId)
        {
            var historique = await _context.Donneeboursiere
                .Where(d => d.ActifId == actifId)
                .OrderBy(d => d.Date)
                .ToListAsync();

            if (historique.Count < 30)
                return BadRequest("Pas assez de données (minimum 30 jours).");

            var predictionService = new PredictionService();
            var predictions = predictionService.ForecastPrices(historique);

            return Ok(predictions);
        }
    }



}
