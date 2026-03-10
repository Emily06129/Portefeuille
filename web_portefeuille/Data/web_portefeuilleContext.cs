using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using web_portefeuille.Models;

namespace web_portefeuille.Data
{
    public class web_portefeuilleContext : DbContext
    {
        public web_portefeuilleContext (DbContextOptions<web_portefeuilleContext> options)
            : base(options)
        {
        }

        public DbSet<web_portefeuille.Models.InvestirViewModel> InvestirViewModel { get; set; } = default!;
    }
}
