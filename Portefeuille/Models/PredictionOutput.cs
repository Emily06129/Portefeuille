using Microsoft.ML.Data;

namespace Portefeuille.Models
{
    public class PredictionOutput
    {
        [VectorType(30)]
        public float[] ForecastedPrices { get; set; }
    }
}
// Cette classe ci permet de stocker le tableau de prix predits grace aux données historiques