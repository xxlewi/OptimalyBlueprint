using Microsoft.AspNetCore.Mvc;

namespace OptimalyBlueprint.Controllers
{
    public class ParentsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ChildLoans()
        {
            return View();
        }
    }
}