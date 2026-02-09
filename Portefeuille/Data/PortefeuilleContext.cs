using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Portefeuille.Models;

namespace Portefeuille.Data
{
    public class PortefeuilleContext : DbContext
    {
        public PortefeuilleContext (DbContextOptions<PortefeuilleContext> options)
            : base(options)
        {
        }

        public DbSet<Portefeuille.Models.Actif> Actif { get; set; } = default!;
    }
}
