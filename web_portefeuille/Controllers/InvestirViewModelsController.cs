using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text.Json;
using web_portefeuille.Models;

namespace web_portefeuille.Controllers
{
    public class InvestirController : Controller
    {// Pour faire des appels http vers l'api 
        private readonly HttpClient _httpClient;

        public InvestirController(IHttpClientFactory factory)
        {
            _httpClient = factory.CreateClient();
        }
        // GET : /Investir/Invest
        public IActionResult Invest()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UtilisateurConnecte")))
            {
                return RedirectToAction("Login", "Account", new { returnUrl = "/Investir/Invest" });
            }
            return View();
        }

        // POST : /Investir/Invest
        [HttpPost]
        public IActionResult Invest(InvestirViewModel model)
        {
            if (model.Budget <= 0)
            {
                ViewBag.Error = "Le budget doit être supérieur à 0.";
                return View(new InvestirViewModel());
            }

            if (model.SymbolesActifs == null || model.SymbolesActifs.Count < 2)
            {
                ViewBag.Error = "Veuillez sélectionner au moins 2 actifs.";
                return View(new InvestirViewModel());
            }

            // Stocke les choix en session et le budget pour les utiliser dans la page de résultats
            HttpContext.Session.SetString("Budget", model.Budget.ToString());
            HttpContext.Session.SetString("Actifs", string.Join(",", model.SymbolesActifs));

            // Redirige vers la page de résultats
            return RedirectToAction("Resultats");
        }

        // GET : /Investir/Resultats
        public IActionResult Resultats()
        {
            // Récupère les choix depuis la session que l'on convertit en string et liste
            var budget = HttpContext.Session.GetString("Budget");
            var actifs = HttpContext.Session.GetString("Actifs")?.Split(",").ToList();

            ViewBag.Budget = budget;
            ViewBag.Actifs = actifs;

            return View();
        }

        // GET : /Investir/Evolution/{symbole}
        public async Task<IActionResult> Evolution(string symbole)
        {
            // Appelle l'API pour récupérer les prédictions de prix pour le symbole donné (ex: AAPL, BTC-USD)
            var response = await _httpClient.GetAsync(
                $"http://localhost:5172/api/DonneesPredites/{symbole}");

            // Lit la réponse JSON de l'API
            var json = await response.Content.ReadAsStringAsync();

            // Désérialise le JSON en liste d'objets DonneesPredites
            // PropertyNameCaseInsensitive : ignore la casse des propriétés (ex: "prixPredit" = "PrixPredit")
            var donnees = JsonSerializer.Deserialize<List<DonneesPredites>>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            // Passe le symbole à la vue pour l'afficher dans le titre du graphe
            ViewBag.Symbole = symbole;

            // Extrait les dates formatées (ex: "16/03") pour l'axe X du graphe
            ViewBag.Dates = donnees?.Select(d => d.DatePrediction.ToString("dd/MM")).ToList();

            // Extrait les prix prédits pour l'axe Y du graphe
            ViewBag.Prix = donnees?.Select(d => d.PrixPredit).ToList();

            return View();
        }


    }
}