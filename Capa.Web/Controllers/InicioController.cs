using Microsoft.AspNetCore.Mvc;

namespace Capa.Web.Controllers
{
    public class InicioController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
