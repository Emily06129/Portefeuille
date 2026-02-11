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
    public class ActifsController : ControllerBase
    {
        private readonly PortefeuilleContext _context;

        public ActifsController(PortefeuilleContext context)
        {
            _context = context;
        }

        // GET: api/Actifs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Actif>>> GetActif()
        {
            return await _context.Actif.ToListAsync();


        }

        // GET: api/Actifs/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Actif>> GetActif(int id)
        {
            var actif = await _context.Actif.Include("ListeDonneeBoursieres")
                .Include("ListeAllocations")
                .FirstOrDefaultAsync(a => a.Id == id);

            if (actif == null)
            {
                return NotFound();
            }

            return actif;
        }

        // PUT: api/Actifs/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutActif(int id, Actif actif)
        {
            if (id != actif.Id)
            {
                return BadRequest();
            }

            _context.Entry(actif).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ActifExists(id))
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

        // POST: api/Actifs
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Actif>> PostActif(Actif actif)
        {
            _context.Actif.Add(actif);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetActif", new { id = actif.Id }, actif);
        }

        // DELETE: api/Actifs/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteActif(int id)
        {
            var actif = await _context.Actif.FindAsync(id);
            if (actif == null)
            {
                return NotFound();
            }

            _context.Actif.Remove(actif);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ActifExists(int id)
        {
            return _context.Actif.Any(e => e.Id == id);
        }
    }
}
