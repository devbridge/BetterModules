using BetterModules.Core.Web.Mvc;
using BetterModules.Sample.Web.Module.Commands.GetModulesList;
using Microsoft.AspNet.Mvc;

namespace BetterModules.Sample.Web.Module.Controllers
{
    [Area("module-bettermoduleswebsample")]
    public class SampleWebController : CoreControllerBase
    {
        public IActionResult Index()
        {
            return Content("Hello World From Sample Controller!");
        }
        
        public IActionResult Test()
        {
            return View();
        }

        public IActionResult ModulesList()
        {
            var command = GetCommand<GetModulesListCommand>();
            var model = command.Execute();

            return View(model);
        }
    }
}
