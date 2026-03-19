namespace Portefeuille.Services
{
    public class MonteCarloService
    {
        private const int NbSimulations = 10_000;

        public (double VaR95, double CVaR95) CalculerRisque(
            double[] poids,
            double[] rendements,
            double[,] covariance)
        {
            int n = poids.Length;
            double[,] L = CholeskyDecomposition(covariance, n);
            var rng = new Random(42);
            var rendsSim = new double[NbSimulations];

            for (int s = 0; s < NbSimulations; s++)
            {
                // Générer des chocs normaux corrélés via Cholesky
                double[] z = GenererNormaux(n, rng);
                double[] choc = new double[n];
                for (int i = 0; i < n; i++)
                    for (int k = 0; k <= i; k++)
                        choc[i] += L[i, k] * z[k];

                // Rendement simulé du portefeuille
                double rend = 0;
                for (int i = 0; i < n; i++)
                    rend += poids[i] * (rendements[i] + choc[i]);

                rendsSim[s] = rend;
            }

            Array.Sort(rendsSim);

            // VaR 95% = perte au 5e percentile
            int idxVaR = (int)(0.05 * NbSimulations);
            double VaR = -rendsSim[idxVaR];

            // CVaR = moyenne des pires 5% de scénarios
            double CVaR = -rendsSim.Take(idxVaR).Average();

            return (Math.Round(VaR * 100, 2), Math.Round(CVaR * 100, 2));
        }

        // Décomposition de Cholesky
        private double[,] CholeskyDecomposition(double[,] A, int n)
        {
            double[,] L = new double[n, n];
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j <= i; j++)
                {
                    double sum = A[i, j];
                    for (int k = 0; k < j; k++)
                        sum -= L[i, k] * L[j, k];

                    L[i, j] = i == j
                        ? Math.Sqrt(Math.Max(sum, 1e-10))
                        : sum / L[j, j];
                }
            }
            return L;
        }

        // Box-Muller : génère des valeurs N(0,1)
        private double[] GenererNormaux(int n, Random rng)
        {
            var result = new double[n];
            for (int i = 0; i < n; i += 2)
            {
                double u1 = 1.0 - rng.NextDouble();
                double u2 = 1.0 - rng.NextDouble();
                double mag = Math.Sqrt(-2.0 * Math.Log(u1));
                result[i] = mag * Math.Cos(2 * Math.PI * u2);
                if (i + 1 < n)
                    result[i + 1] = mag * Math.Sin(2 * Math.PI * u2);
            }
            return result;
        }
    }
}