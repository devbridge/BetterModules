using System;
using BetterModules.Core.Infrastructure.Commands;
using BetterModules.Core.Web.Modules;
using BetterModules.Core.Web.Modules.Registration;
using Microsoft.Framework.DependencyInjection;
using Xunit;

namespace BetterModules.Core.Web.Tests.Modules
{
    public class WebModuleDescriptorTests
    {
        [Fact]
        public void Should_Return_Correct_AreaName()
        {
            var descriptor = new TestWebModuleDescriptor();

            Assert.Equal(descriptor.AreaName, "module-testwebmodule");
        }

        //[Fact]
        //public void Should_Register_ModuleControllers_Correctly()
        //{
        //    var descriptor = new TestWebModuleDescriptor();
        //    var context = new WebModuleRegistrationContext(descriptor);
        //    var containerBuilder = new ContainerBuilder();

        //    var controllerExtensions = new Mock<IControllerExtensions>();
        //    controllerExtensions
        //        .Setup(ce => ce.GetControllerTypes(It.IsAny<Assembly>()))
        //        .Returns<Assembly>(assembly => new[] { typeof(TestController1), typeof(TestController2) });

        //    descriptor.RegisterModuleControllers(context, containerBuilder, controllerExtensions.Object);

        //    // Routes registration
        //    Assert.Equal(context.Routes.Count, 1);
        //    var route = (Route)context.Routes[0];
        //    Assert.Equal(route.Url, "module-testwebmodule/{controller}/{action}");
        //    Assert.Equal(route.DataTokens["area"], "module-testwebmodule");

        //    // Types registration
        //    var container = containerBuilder.Build();

        //    var controller1 = container.Resolve(typeof(TestController1));
        //    var controller2 = container.Resolve(typeof(TestController2));

        //    TestController3 controller3 = null;
        //    try
        //    {
        //        controller3 = container.Resolve<TestController3>();
        //    }
        //    catch
        //    {
        //    }

        //    Assert.NotNull(controller1);
        //    Assert.NotNull(controller2);
        //    Assert.Null(controller3);
        //}

        [Fact]
        public void Should_Register_RegisterModuleCommands_Correctly()
        {
            var descriptor = new TestWebModuleDescriptor();
            var context = new WebModuleRegistrationContext(descriptor);
            var services = new ServiceCollection();

            descriptor.RegisterModuleCommands(context, services);

            var provider = services.BuildServiceProvider();

            var commandin = provider.GetService<TestCommandIn>();
            var commandout = provider.GetService<TestCommandOut>();
            var commandinout = provider.GetService<TestCommandInOut>();

            Assert.NotNull(commandin);
            Assert.NotNull(commandout);
            Assert.NotNull(commandinout);
        }

        [Fact]
        public void Should_Return_Correct_RegistrationContext()
        {
            var descriptor = new TestWebModuleDescriptor();
            var context = descriptor.CreateRegistrationContext();

            Assert.NotNull(context);
            Assert.Equal(context.GetType(), typeof(WebModuleRegistrationContext));
        }

        #region CLASSES FOR TESTS

        private class TestWebModuleDescriptor : WebModuleDescriptor
        {
            public override string Name => "testWebModule";

            public override string Description => "Test web module";
        }

        public class TestCommandIn : CoreCommandBase, ICommandIn<int>
        {
            public void Execute(int request)
            {
                throw new NotImplementedException();
            }
        }

        public class TestCommandOut : CoreCommandBase, ICommandOut<int>
        {
            public int Execute()
            {
                throw new NotImplementedException();
            }
        }

        public class TestCommandInOut : CoreCommandBase, ICommand<int, int>
        {
            public int Execute(int request)
            {
                throw new NotImplementedException();
            }
        }

        #endregion
    }
}
