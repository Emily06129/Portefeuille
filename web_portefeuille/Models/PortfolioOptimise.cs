namespace web_portefeuille.Models
{
    public class PortfolioOptimise
    {
        public string NiveauRisque { get; set; }
        public Dictionary<string, double> Poids { get; set; }
        public double RendementAttendu { get; set; }
        public double Volatilite { get; set; }
        public double SharpeRatio { get; set; }
        public double VaR95 { get; set; }
        public double CVaR95 { get; set; }
    }
}