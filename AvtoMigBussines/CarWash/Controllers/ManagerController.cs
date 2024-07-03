using Microsoft.AspNetCore.Mvc;

namespace AvtoMigBussines.CarWash.Controllers
{
    public class ManagerController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
