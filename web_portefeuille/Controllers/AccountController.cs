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
            var json = JsonSerializer.Serialize(client);// Convertir un client en JSON
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(
                "http://localhost:5172/Portefeuille/Clients/register",
                content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Login");
            }

            ViewBag.Error = "Erreur lors de l'inscription";
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
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(
                "https://localhost:5172/Portefeuille/Clients/login",
                content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Email ou mot de passe incorrect";
            return View();
        }
    }
}