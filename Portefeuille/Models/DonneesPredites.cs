using Microsoft.ML.Data;

namespace Portefeuille.Models
{
    public class DonneesPredites
    {
        [VectorType(30)]
        public float[] ForecastedPrices { get; set; }
    }
}