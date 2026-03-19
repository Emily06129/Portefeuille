namespace Portefeuille.Models
{
    public class SauvegarderPortfolioDto
    {
        public string NiveauRisque { get; set; } = "";
        public double SharpeRatio { get; set; }
        public double RendementAttendu { get; set; }
        public double Volatilite { get; set; }
        public double VaR95 { get; set; }
        public double CVaR95 { get; set; }
        public double Budget { get; set; }
        public Dictionary<string, double> Poids { get; set; } = new();
        public int ClientId { get; set; }
    }
}
