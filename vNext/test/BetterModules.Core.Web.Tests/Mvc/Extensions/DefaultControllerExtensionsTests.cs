using System.Linq;
using System.Reflection;
using BetterModules.Core.Environment.Assemblies;
using BetterModules.Core.Web.Mvc.Extensions;
using Microsoft.AspNet.Mvc;
using Moq;
using Xunit;

namespace BetterModules.Core.Web.Tests.Mvc.Extensions
{
    public class DefaultControllerExtensionsTests
    {
        [Fact]
        public void ShouldReturn_ControllerTypesList_Correctly()
        {
            var controllerExt = GetControllerTExtensions();
            var controllers = controllerExt.GetControllerTypes(GetType().Assembly);

            Assert.NotNull(controllers);
            var controllersList = controllers.ToList();
            Assert.Equal(controllersList.Count, 1);
            Assert.Equal(controllersList[0], typeof(PublicTestController));
        }

        [Fact]
        public void ShouldReturn_Correct_ControllerName()
        {
            var controllerExt = GetControllerTExtensions();
            var controllerName = controllerExt.GetControllerName(typeof(PublicNestedTestController));

            Assert.Equal(controllerName, "PublicNestedTest");
        }

        [Fact]
        public void ShouldReturn_Correct_NonControllerName()
        {
            var controllerExt = GetControllerTExtensions();
            var controllerName = controllerExt.GetControllerName(typeof(int));

            Assert.Null(controllerName);
        }

        [Fact]
        public void ShouldReturn_Correct_Controller_Actions()
        {
            var controllerExt = GetControllerTExtensions();
            var actions = controllerExt.GetControllerActions(typeof(WebTestController));

            Assert.NotNull(actions);
            Assert.Equal(1, actions.Count());
            Assert.Equal("TestAction", actions.First().Name);
        }

        [Fact]
        public void ShouldReturn_Correct_GenericController_Actions()
        {
            var controllerExt = GetControllerTExtensions();
            var actions = controllerExt.GetControllerActions<WebTestController>();

            Assert.NotNull(actions);
            Assert.Equal(1, actions.Count());
            Assert.Equal("TestAction", actions.First().Name);
        }

        private DefaultControllerExtensions GetControllerTExtensions()
        {
            var types = new[]
            {
                typeof (AbstractTestController),
                typeof (PrivateTestController),
                typeof (PublicTestController),
                typeof (PublicNestedTestController),
                typeof (Controller),
                typeof (DefaultControllerExtensionsTests)
            };

            var assemblyLoader = new Mock<IAssemblyLoader>();
            assemblyLoader.Setup(a => a.GetLoadableTypes(It.IsAny<Assembly>())).Returns<Assembly>(a => types);

            var controllerExt = new DefaultControllerExtensions(assemblyLoader.Object);

            return controllerExt;
        }

        private class PrivateTestController : Controller
        {
            public void Execute(ActionContext actionContext)
            {
            }
        }

        public class PublicNestedTestController : Controller
        {
            public void Execute(ActionContext actionContext)
            {
            }
        }

        public class WebTestController : Controller
        {
            public IActionResult TestAction()
            {
                throw new System.NotImplementedException();
            }
        }
    }

    public abstract class AbstractTestController : Controller
    {
        public void Execute(ActionContext actionContext)
        {
        }
    }

    public class PublicTestController : Controller
    {
        public void Execute(ActionContext actionContext)
        {
        }
    }
}
