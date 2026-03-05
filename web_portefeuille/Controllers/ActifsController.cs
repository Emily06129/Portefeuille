using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using web_portefeuille.Models;

namespace web_portefeuille.Controllers
{
    public class ActifsController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiUrl = "http://localhost:5172/api";

        public ActifsController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        // GET: /Actifs
        public async Task<IActionResult> Index()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_apiUrl}/actifs");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var actifs = JsonSerializer.Deserialize<List<Actif>>(json,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    return View(actifs ?? new List<Actif>());
                }

                ViewBag.Erreur = "L'API a retourné une erreur.";
                return View(new List<Actif>());
            }
            catch (Exception ex)
            {
                ViewBag.Erreur = $"API inaccessible. Vérifiez que l'API tourne. ({ex.Message})";
                return View(new List<Actif>());
            }
        }

        // GET: /Actifs/Details/5
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                // Appel 1 : récupère l'actif
                var responseActif = await _httpClient.GetAsync($"{_apiUrl}/actifs/{id}");
                if (!responseActif.IsSuccessStatusCode)
                    return RedirectToAction("Index");

                var jsonActif = await responseActif.Content.ReadAsStringAsync();
                var actif = JsonSerializer.Deserialize<Actif>(jsonActif,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                // Appel 2 : récupère l'historique des prix
                var responseDonnees = await _httpClient
                    .GetAsync($"{_apiUrl}/donneeboursieres/actif/{id}?limit=10000");

                List<Donneeboursiere> donnees = new();
                if (responseDonnees.IsSuccessStatusCode)
                {
                    var jsonDonnees = await responseDonnees.Content.ReadAsStringAsync();
                    donnees = JsonSerializer.Deserialize<List<Donneeboursiere>>(jsonDonnees,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                        ?? new List<Donneeboursiere>();
                }

                ViewBag.Actif = actif;
                ViewBag.Donnees = donnees;

                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Erreur = $"Erreur : {ex.Message}";
                return View();
            }
        }
    }
}