using System.Web.Mvc;
using BetterModules.Core.Web.Mvc;
using BetterModules.Sample.Web.Module.Commands.GetModulesList;

namespace BetterModules.Sample.Web.Module.Controllers
{
    public class SampleWebController : CoreControllerBase
    {
        public ActionResult Index()
        {
            return Content("Hello World From Sample Controller!");
        }
        
        public ActionResult Test()
        {
            return View();
        }

        public ActionResult ModulesList()
        {
            var command = GetCommand<GetModulesListCommand>();
            var model = command.Execute();

            return View(model);
        }
    }
}
