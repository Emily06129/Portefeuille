using System.ComponentModel.DataAnnotations;

namespace Portefeuille.Models
{
    public class Client
    {
        public int Id { get; set; }
        public string? Nom { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public float Budget { get; set; }

        public virtual ICollection<Portfolio_client>? ListePortfolio_Client { get; set; }


    }
}
