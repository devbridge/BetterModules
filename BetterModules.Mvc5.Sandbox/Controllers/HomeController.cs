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
            using (var scope = ContextScopeProvider.CreateChildContainer())
            {
                string postfix = "__1";

                TestItemCategory category = new TestItemCategory();
                category.Name = "test_pm" + postfix;

                TestItemModel item = new TestItemModel();
                item.Name = "big test" + postfix;
                item.Category = category;

                IRepository repository = scope.Resolve<IRepository>();
                IUnitOfWork unitOfWork = scope.Resolve<IUnitOfWork>();

                repository.Save(item);
                unitOfWork.Commit();

                item.Name = "ups";
                item.Version = 3;
                repository.Save(item);
                unitOfWork.Commit();

                var l = repository.AsQueryable<TestItemCategory>().ToList();
            }

            return View();
        }
    }
}