namespace Portefeuille.Models
{
    public class PortfolioOptimise
    {
        public string NiveauRisque { get; set; }
        public Dictionary<string, double>? Poids { get; set; }
        public double RendementAttendu { get; set; }   // mensuel
        public double Volatilite { get; set; }          // mensuel
        public double SharpeRatio { get; set; }
        public double VaR95 { get; set; }               // Monte Carlo
        public double CVaR95 { get; set; }              // Monte Carlo
    }
}