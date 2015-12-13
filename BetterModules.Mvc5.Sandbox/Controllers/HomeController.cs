using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using Autofac;
using BetterModules.Core;
using BetterModules.Core.DataAccess;
using BetterModules.Core.DataAccess.DataContext;
using BetterModules.Core.Dependencies;
using BetterModules.Sample.Module.Models;

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