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

        public DbSet<Portefeuille.Models.Actif> Actif { get; set; }
        public DbSet<Portefeuille.Models.Donneeboursiere> Donneeboursiere { get; set; } = default!;
        public DbSet<Portefeuille.Models.Client> Client { get; set; } = default!;
        public DbSet<Portefeuille.Models.Allocation> Allocation { get; set; } = default!;
        public DbSet<Portefeuille.Models.Portfolio_client> Portfolio_client { get; set; } = default!;
       
        
    }
}
