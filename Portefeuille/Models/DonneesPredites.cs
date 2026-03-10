using Microsoft.ML.Data;

namespace Portefeuille.Models
{
    public class DonneesPredites
    {
        [VectorType(7)]
        public float[] ForecastedPrices { get; set; }
    }
}