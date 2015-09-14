using BetterModules.Sample.Module.Models;
using Microsoft.AspNet.Mvc;

namespace BetterModules.Mvc6.Sandbox.Controllers
{
    public class HomeController : Controller
    {
        private readonly ICalculator _calculator;

        public HomeController(ICalculator calculator)
        {
            _calculator = calculator;
        }

        public IActionResult Index()
        {
            var result = _calculator.FindRoots(1, 4, 4);
            return View();
        }
    }
}
