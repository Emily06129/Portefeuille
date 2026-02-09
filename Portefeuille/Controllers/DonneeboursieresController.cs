using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Portefeuille.Data;
using Portefeuille.Models;

namespace Portefeuille.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DonneeboursieresController : ControllerBase
    {
        private readonly PortefeuilleContext _context;

        public DonneeboursieresController(PortefeuilleContext context)
        {
            _context = context;
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
    }
}
