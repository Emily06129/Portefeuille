using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace web_portefeuille.Controllers
{
    public class ConnexionController : Controller
    {
        // GET: ConnexionController
        public ActionResult Index()
        {
            return View();
        }

        // GET: ConnexionController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ConnexionController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ConnexionController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ConnexionController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ConnexionController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ConnexionController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ConnexionController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
