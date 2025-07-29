using Microsoft.AspNetCore.Mvc;

namespace OptimalyBlueprint.Controllers
{
    public class ReservationsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Create()
        {
            return View();
        }

        public IActionResult Manage()
        {
            return View();
        }
    }
}