using System.Collections.Generic;
using System.Web;
using System.Web.Routing;

using BetterModules.Core.Tests.TestHelpers;
using BetterModules.Core.Web.Environment.Host;
using BetterModules.Core.Web.Modules.Registration;

using BetterModules.Events;

using Moq;

using NUnit.Framework;

namespace BetterModules.Core.Web.Tests.Environment.Host
{
    [TestFixture]
    public class DefaultWebApplicationHostTests : TestBase
    {
        private HttpApplication application;
        private bool eventFired;

        [Test]
        public void ShouldExecute_Init_Correctly()
        {
            eventFired = false;
            var routesRegistered = false;

            using (var fakeProvider = new ContextScopeProviderHelper())
            {

                var registration = new Mock<IWebModulesRegistration>();
                registration
                    .Setup(r => r.RegisterKnownModuleRoutes(It.IsAny<RouteCollection>()))
                    .Callback<RouteCollection>(rc => routesRegistered = true);
                fakeProvider.RegisterFakeServiceInstance(registration.Object, typeof (IWebModulesRegistration));

                var host = new UtilityHost();
                CreateApplication();

                WebCoreEvents.Instance.HostStart += Instance_Fired;
                host.Init(application);
                WebCoreEvents.Instance.HostStart -= Instance_Fired;
            }
            Assert.IsTrue(eventFired);
            Assert.IsTrue(routesRegistered);
        }
        
        [Test]
        public void ShouldExecute_Dispose_Correctly()
        {
            var host = new UtilityHost();
            CreateApplication();
            eventFired = false;
            host.Init(application);
            WebCoreEvents.Instance.HostStop += Instance_Fired;
            host.Dispose();
            WebCoreEvents.Instance.HostStop -= Instance_Fired;

            Assert.IsTrue(eventFired);
        }
        
        [Test]
        public void ShouldExecute_OnApplicationError_Correctly()
        {
            var host = CreateHost();
            CreateApplication();
            eventFired = false;

            WebCoreEvents.Instance.HostError += Instance_Fired;
            host.OnApplicationError(application);
            WebCoreEvents.Instance.HostError -= Instance_Fired;

            Assert.IsTrue(eventFired);
        }
        
        [Test]
        public void ShouldExecute_OnAuthenticateRequest_Correctly()
        {
            var host = CreateHost();
            CreateApplication();
            eventFired = false;
            
            WebCoreEvents.Instance.HostAuthenticateRequest += Instance_Fired;
            host.OnAuthenticateRequest(application);
            WebCoreEvents.Instance.HostAuthenticateRequest -= Instance_Fired;

            Assert.IsTrue(eventFired);
        }

        void Instance_Fired(SingleItemEventArgs<HttpApplication> args)
        {
            Assert.IsNotNull(args);
            Assert.IsNotNull(args.Item);
            Assert.AreEqual(args.Item, application);
            eventFired = true;
        }

        private IWebApplicationHost CreateHost()
        {
            var host = new UtilityHost();
            return host;
        }

        private void CreateApplication()
        {
            application = new Mock<HttpApplication>().Object;
        }

        private class EventTestApplicationAutoHost : DefaultWebApplicationAutoHost
        {
            public List<string> Results = new List<string>();

            public override void OnAuthenticateRequest(HttpApplication application)
            {
                Results.Add("OnAuthenticateRequest");
            }

            public override void OnBeginRequest(HttpApplication application)
            {
                Results.Add("OnBeginRequest");
            }

            public override void OnEndRequest(HttpApplication application)
            {
                Results.Add("OnEndRequest");
            }

            public override void OnApplicationError(HttpApplication application)
            {
                Results.Add("OnApplicationError");
            }
        }
    }
}
