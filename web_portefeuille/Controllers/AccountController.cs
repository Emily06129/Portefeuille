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

        [HttpPost]
        public async Task<IActionResult> Register(Client client)
        {
            var json = JsonSerializer.Serialize(client);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(
                "http://localhost:5172/api/Clients/register",
                content);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Login");
            }

            ViewBag.Error = $"Erreur {(int)response.StatusCode}: {responseBody}";
            return View();
        }

        // =========================
        // PAGE CONNEXION
        // =========================
        [HttpGet]
        public IActionResult Login(string returnUrl = null) // 👈 une seule méthode GET
        {

            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(Client client, string returnUrl = null) // 👈 returnUrl ajouté ici
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
                HttpContext.Session.SetString("UtilisateurConnecte", client.Email); 

                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);

                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = $"Erreur {(int)response.StatusCode}: {responseBody}";
            return View();
        }
    }
}