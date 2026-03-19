using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Portefeuille.Models
{
    public class Portfolio_client
    {
        public int Id { get; set; }

        // Niveau de risque choisi par le client
        [Required]
        public string NiveauRisque { get; set; } = "";  // "Défensif", "Équilibré"...

        // Métriques financières
        public double ScoreSharp { get; set; }
        public double RendementPrevu { get; set; }
        public double RisqueVolatilite { get; set; }
        public double VaR95 { get; set; }
        public double CVaR95 { get; set; }

        // Budget investi
        public double Budget { get; set; }

        // Poids stockés en JSON : {"AAPL":0.35,"BTC-USD":0.20,...}
        public string PoidsJson { get; set; } = "";

        public DateTime DateCreation { get; set; } = DateTime.UtcNow;

        [ForeignKey("Client")]
        public int ClientId { get; set; }
        public virtual Client? Client { get; set; }
    }
}