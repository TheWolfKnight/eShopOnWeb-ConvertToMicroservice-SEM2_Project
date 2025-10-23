using Microsoft.AspNetCore.Mvc;

namespace CatalogMicroservice.API.Controllers
{
    public class CatalogAPI : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
