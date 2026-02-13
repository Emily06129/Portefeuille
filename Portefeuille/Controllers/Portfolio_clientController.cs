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
    public class Portfolio_clientController : ControllerBase
    {
        private readonly PortefeuilleContext _context;

        public Portfolio_clientController(PortefeuilleContext context)
        {
            _context = context;
        }

        // GET: api/Portfolio_client
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Portfolio_client>>> GetPortfolio_client()
        {
            return await _context.Portfolio_client.ToListAsync();
        }

        // GET: api/Portfolio_client/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Portfolio_client>> GetPortfolio_client(int id)
        {
            var portfolio_client = await _context.Portfolio_client.Include("ListeActifs")
               .Include("ListeActifs.ListeDonneeBoursieres")
               .Include("ListeActifs.ListeAllocations")
               .FirstOrDefaultAsync(p => p.Id == id);
            if (portfolio_client == null)
            {
                return NotFound();
            }

            return portfolio_client;
        }

        // PUT: api/Portfolio_client/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPortfolio_client(int id, Portfolio_client portfolio_client)
        {
            if (id != portfolio_client.Id)
            {
                return BadRequest();
            }

            _context.Entry(portfolio_client).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!Portfolio_clientExists(id))
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

        // POST: api/Portfolio_client
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Portfolio_client>> PostPortfolio_client(Portfolio_client portfolio_client)
        {
            _context.Portfolio_client.Add(portfolio_client);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPortfolio_client", new { id = portfolio_client.Id }, portfolio_client);
        }

        // DELETE: api/Portfolio_client/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePortfolio_client(int id)
        {
            var portfolio_client = await _context.Portfolio_client.FindAsync(id);
            if (portfolio_client == null)
            {
                return NotFound();
            }

            _context.Portfolio_client.Remove(portfolio_client);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool Portfolio_clientExists(int id)
        {
            return _context.Portfolio_client.Any(e => e.Id == id);
        }
    }
}
