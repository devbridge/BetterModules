using System.Web.Routing;
using BetterModules.Core.Web.Mvc.Routes;
using NUnit.Framework;

namespace BetterModules.Core.Web.Tests.Mvc.Routes
{
    [TestFixture]
    public class DefaultRouteTableTests
    {
        [Test]
        public void Should_Initialize_RouteTable_Properties_Correctly()
        {
            var routesCollection = new RouteCollection();
            var table = new DefaultRouteTable(routesCollection);

            Assert.AreEqual(table.Routes, routesCollection);
        }
    }
}
