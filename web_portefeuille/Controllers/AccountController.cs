using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;
using web_portefeuille.Models;

namespace web_portefeuille.Controllers
{
    public class AccountController : Controller
    {
        private readonly HttpClient _httpClient;

        public AccountController(IHttpClientFactory factory)
        {
            _httpClient = factory.CreateClient();
        }

        // =========================
        // PAGE INSCRIPTION
        // =========================

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        //On utilise un Model typé (Client)
        //ASP.NET fait automatiquement :
        //collection["Email"]
        //On a directement :
        //client.Email
        [HttpPost]
        public async Task<IActionResult> Register(Client client)
        {
            var json = JsonSerializer.Serialize(client);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(
                "http://localhost:5172/api/Clients/register",
                content);

            // Lire le détail de l'erreur
            var responseBody = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Login");
            }

            // Afficher le vrai message d'erreur
            ViewBag.Error = $"Erreur {(int)response.StatusCode}: {responseBody}";
            return View();
        }
        // =========================
        // PAGE CONNEXION
        // =========================

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        
        [HttpPost]
        public async Task<IActionResult> Login(Client client)
        {
            var json = JsonSerializer.Serialize(client);
            Console.WriteLine("JSON envoyé : " + json);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(
                "http://localhost:5172/api/Clients/login",
                content);
            var responseBody = await response.Content.ReadAsStringAsync();
            Console.WriteLine("Réponse : " + (int)response.StatusCode + " " + responseBody);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.Error = $"Erreur {(int)response.StatusCode}: {responseBody}";
            return View();
        }
    }
}