using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Transforms.TimeSeries;
using Portefeuille.Models;

namespace Portefeuille.Services
{
    public class PredictionService
    {
        //L’initialisation mlContext crée un environnement ML.NET qui peut être partagé entre les objets
        private readonly MLContext mlContext = new MLContext(seed: 42);
        public float[] ForecastPrices(List<Donneeboursiere> historique, int nbJours = 7)
        {
            //ml lit les données boursieres
            var dataView = mlContext.Data.LoadFromEnumerable(historique);


            // 2. Configuration du pipeline de transformation pour les séries temporelles

            var forecastingPipeline = mlContext.Forecasting.ForecastBySsa(
            outputColumnName: "ForecastedPrices",
            inputColumnName: "Cloture",
            windowSize: 7, // Chaque echantillon est analysé par semaine
            seriesLength: 30, // Nombre d'echantillons de données utilisés pour entraîner le modèle
            horizon: nbJours,
            trainSize: historique.Count

        );
            // Entraînement du modèle

            var model = forecastingPipeline.Fit(dataView);

            // Prédiction des prix futurs
            var forecastingEngine = model.CreateTimeSeriesEngine<Donneeboursiere, DonneesPredites>(mlContext);
            var forecast = forecastingEngine.Predict();

            return forecast.ForecastedPrices;
        }
    }
}
