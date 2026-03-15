using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;
using web_portefeuille.Models;

namespace web_portefeuille.Controllers
{
    public class AccountController : Controller
    {
        // Client HTTP pour appeler l'API externe (Portefeuille API)
        private readonly HttpClient _httpClient;

        // Injection du factory pour créer le client HTTP
        public AccountController(IHttpClientFactory factory)
        {
            _httpClient = factory.CreateClient();
        }

        // =========================
        // PAGE INSCRIPTION
        // =========================

        // Affiche le formulaire d'inscription
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // Traite le formulaire d'inscription soumis
        [HttpPost]
        public async Task<IActionResult> Register(Client client)
        {
            // Convertit l'objet client en JSON pour l'envoyer à l'API
            var json = JsonSerializer.Serialize(client);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Envoie la requête POST à l'API d'inscription
            var response = await _httpClient.PostAsync(
                "http://localhost:5172/api/Clients/register",
                content);

            var responseBody = await response.Content.ReadAsStringAsync();

            // Si l'inscription réussit, redirige vers la page de connexion
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Login");
            }

            // Sinon, affiche le message d'erreur retourné par l'API
            ViewBag.Error = $"Erreur {(int)response.StatusCode}: {responseBody}";
            return View();
        }

        // =========================
        // PAGE CONNEXION
        // =========================

        // Affiche le formulaire de connexion
        // returnUrl : page où l'utilisateur voulait aller avant d'être redirigé ici
        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            // Passe le returnUrl à la vue pour le conserver dans le formulaire
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        // Traite le formulaire de connexion soumis
        // returnUrl : récupéré depuis le champ caché du formulaire
        [HttpPost]
        public async Task<IActionResult> Login(Client client, string returnUrl = null)
        {
            // Convertit les identifiants en JSON pour l'envoyer à l'API
            var json = JsonSerializer.Serialize(client);
            Console.WriteLine("JSON envoyé : " + json);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Envoie la requête POST à l'API de connexion
            var response = await _httpClient.PostAsync(
                "http://localhost:5172/api/Clients/login",
                content);

            var responseBody = await response.Content.ReadAsStringAsync();
            Console.WriteLine("Réponse : " + (int)response.StatusCode + " " + responseBody);

            if (response.IsSuccessStatusCode)
            {
                // Connexion réussie : enregistre l'email en session pour savoir qui est connecté
                HttpContext.Session.SetString("UtilisateurConnecte", client.Email);

                // Si l'utilisateur venait d'une page protégée, on l'y redirige
                // Url.IsLocalUrl évite les redirections malveillantes vers des sites externes
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);

                // Sinon redirige vers la page d'accueil
                return RedirectToAction("Index", "Home");
            }

            // Connexion échouée : affiche le message d'erreur de l'API
            ViewBag.Error = $"Erreur {(int)response.StatusCode}: {responseBody}";
            return View();
        }
    }
}