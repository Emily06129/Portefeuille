using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using web_portefeuille.Models;

namespace web_portefeuille.Controllers
{
    public class InvestirController : Controller
    {
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


            return View();
        }
    }
}