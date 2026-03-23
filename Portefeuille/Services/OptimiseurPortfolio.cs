using Portefeuille.Models;

namespace Portefeuille.Services
{
    public class OptimiseurPortfolio
    {
        private const double TauxSansRisque = 0.04 / 12; // 4% annuel → mensuel
        private const int NbSimulations = 50_000;

        /// <summary>
        /// Point d'entrée principal — génère 5 portefeuilles optimaux
        /// </summary>
        public List<PortfolioOptimise> GenererPortefeuilles(
            Dictionary<string, float[]> predictions,   // sortie de PredictionService
            Dictionary<string, List<Donneeboursiere>> historiques) // depuis ta BDD
        {
            var symboles = predictions.Keys.ToList();
            int n = symboles.Count;

            // 1. Rendements prédits : (prixJour30 - prixActuel) / prixActuel
            double[] rendementsPredits = CalculerRendementsPredits(predictions, historiques, symboles);

            // 2. Matrice de covariance depuis l'historique
            double[,] covariance = CalculerCovariance(historiques, symboles);

            // 3. Simuler 50 000 portefeuilles aléatoires (Monte Carlo Markowitz)
            var simulations = SimulerPortefeuilles(rendementsPredits, covariance, n);

            // 4. Extraire 5 profils de risque sur la frontière efficiente
            return ExtraireProfilsRisque(simulations, symboles);
        }

        // ÉTAPE 1 : Rendements depuis les prédictions ML.NET

        private double[] CalculerRendementsPredits(
            Dictionary<string, float[]> predictions,
            Dictionary<string, List<Donneeboursiere>> historiques,
            List<string> symboles)
        {
            return symboles.Select(sym =>
            {
                var prixActuel = (double)historiques[sym].Last().Cloture;
                var prixPredit = (double)predictions[sym].Last(); // jour 30
                return (prixPredit - prixActuel) / prixActuel;
            }).ToArray();
        }


        // ÉTAPE 2 : Matrice de covariance

        private double[,] CalculerCovariance(
            Dictionary<string, List<Donneeboursiere>> historiques,
            List<string> symboles)
        {
            int n = symboles.Count;

            // Rendements journaliers pour chaque actif
            var rendJournaliers = symboles.Select(sym =>
            {
                var prix = historiques[sym]
                    .OrderBy(d => d.Date)
                    .Select(d => (double)d.Cloture)
                    .ToArray();

                // Rendement(t) = (Prix(t) - Prix(t-1)) / Prix(t-1)
                return prix.Skip(1)
                           .Select((p, i) => (p - prix[i]) / prix[i])
                           .ToArray();
            }).ToList();

            // Aligner sur la longueur minimale
            int minLen = rendJournaliers.Min(r => r.Length);
            var rends = rendJournaliers
                .Select(r => r.TakeLast(minLen).ToArray())
                .ToList();

            // Calculer covariance[i,j]
            double[,] cov = new double[n, n];
            for (int i = 0; i < n; i++)
            {
                double moyI = rends[i].Average();
                for (int j = 0; j < n; j++)
                {
                    double moyJ = rends[j].Average();
                    double somme = 0;
                    for (int t = 0; t < minLen; t++)
                        somme += (rends[i][t] - moyI) * (rends[j][t] - moyJ);

                    // Annualiser × 252 jours de bourse
                    cov[i, j] = (somme / (minLen - 1)) * 252;
                }
            }
            return cov;
        }


        // ÉTAPE 3 : Simulations de portefeuilles

        private record SimPortfolio(
            double[] Poids,
            double Rendement,
            double Volatilite,
            double Sharpe);

        private List<SimPortfolio> SimulerPortefeuilles(
            double[] rendements, double[,] cov, int n)
        {
            var rng = new Random(42);
            var resultats = new List<SimPortfolio>(NbSimulations);

            for (int s = 0; s < NbSimulations; s++)
            {
                double[] poids = GenererPoidsAleatoires(n, rng);

                // Rendement portefeuille = Σ(poids_i * rendement_i)
                double rend = 0;
                for (int i = 0; i < n; i++)
                    rend += poids[i] * rendements[i];

                // Variance portefeuille = w' * Σ * w
                double variance = 0;
                for (int i = 0; i < n; i++)
                    for (int j = 0; j < n; j++)
                        variance += poids[i] * poids[j] * cov[i, j];

                double vol = Math.Sqrt(Math.Abs(variance));
                double sharpe = vol > 1e-8
                    ? (rend - TauxSansRisque) / vol
                    : 0;

                resultats.Add(new SimPortfolio(poids, rend, vol, sharpe));
            }
            return resultats;
        }

        private double[] GenererPoidsAleatoires(int n, Random rng)
        {
            // Distribution de Dirichlet simplifiée : exponentielles normalisées
            double[] raw = Enumerable.Range(0, n)
                .Select(_ => -Math.Log(rng.NextDouble() + 1e-10))
                .ToArray();
            double somme = raw.Sum();
            return raw.Select(r => r / somme).ToArray();
        }


        // ÉTAPE 4 : Extraction des 5 profils

        private List<PortfolioOptimise> ExtraireProfilsRisque(
            List<SimPortfolio> sims, List<string> symboles)
        {
            var tries = sims.OrderBy(s => s.Volatilite).ToList();
            int total = tries.Count;

            // Sélection par percentile de volatilité + max Sharpe
            var profils = new List<(string Nom, SimPortfolio Sim)>
            {
                ("Très prudent",     tries[(int)(total * 0.05)]),
                ("Prudent", tries[(int)(total * 0.25)]),
                ("Équilibré",    sims.MaxBy(s => s.Sharpe)!),   // Max Sharpe
                ("Dynamique",    tries[(int)(total * 0.75)]),
                ("Agressif",     tries[(int)(total * 0.95)])
            };

            return profils.Select(p =>
            {
                var poids = new Dictionary<string, double>();
                for (int i = 0; i < symboles.Count; i++)
                    poids[symboles[i]] = Math.Round(p.Sim.Poids[i], 4);

                return new PortfolioOptimise
                {
                    NiveauRisque = p.Nom,
                    Poids = poids,
                    RendementAttendu = Math.Round(p.Sim.Rendement * 100, 2),
                    Volatilite = Math.Round(p.Sim.Volatilite * 100, 2),
                    SharpeRatio = Math.Round(p.Sim.Sharpe, 4)
                };
            }).ToList();
        }

        // Méthode publique pour exposer la covariance au controller
        public double[,] GetCovariance(
            Dictionary<string, List<Donneeboursiere>> historiques,
            List<string> symboles)
        {
            return CalculerCovariance(historiques, symboles);
        }
    }
}