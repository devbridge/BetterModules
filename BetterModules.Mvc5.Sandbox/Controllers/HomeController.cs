using System.Web.Mvc;

namespace BetterModules.Mvc5.Sandbox.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}