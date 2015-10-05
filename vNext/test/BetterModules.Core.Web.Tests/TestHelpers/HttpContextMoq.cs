using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Http.Features;
using Moq;

namespace BetterModules.Core.Web.Tests.TestHelpers
{
    public class HttpContextMoq
    {
        public Mock<HttpContext> MockContext { get; set; }

        public Mock<HttpRequest> MockRequest { get; set; }

        public Mock<HttpResponse> MockResponse { get; set; }

        public Mock<ISession> MockSession { get; set; }

        public Mock<ClaimsPrincipal> MockUser { get; set; }

        public Mock<IIdentity> MockIdentity { get; set; }

        public Mock<IDictionary<object, object>> MockItems { get; set; }

        public HttpContext HttpContextBase { get; set; }

        public HttpRequest HttpRequestBase { get; set; }

        public HttpResponse HttpResponseBase { get; set; }

        public HttpContextMoq()
        {
            CreateBaseMocks();
            SetupNormalRequestValues();
        }

        public HttpContextMoq CreateBaseMocks()
        {
            MockContext = new Mock<HttpContext>();
            MockRequest = new Mock<HttpRequest>();
            MockResponse = new Mock<HttpResponse>();
            MockSession = new Mock<ISession>();
            MockItems = new Mock<IDictionary<object, object>>();

            MockContext.Setup(ctx => ctx.Request).Returns(MockRequest.Object);
            MockContext.Setup(ctx => ctx.Response).Returns(MockResponse.Object);
            MockContext.Setup(ctx => ctx.Session).Returns(MockSession.Object);
            MockContext.Setup(ctx => ctx.Items).Returns(MockItems.Object);

            HttpContextBase = MockContext.Object;
            HttpRequestBase = MockRequest.Object;
            HttpResponseBase = MockResponse.Object;

            return this;
        }


        public HttpContextMoq SetupNormalRequestValues()
        {
            var MockUser = new Mock<ClaimsPrincipal>();
            var MockIdentity = new Mock<IIdentity>();

            MockContext.Setup(context => context.User).Returns(MockUser.Object);
            MockUser.Setup(context => context.Identity).Returns(MockIdentity.Object);

            //MockRequest.Setup(request => request.Url).Returns(new Uri("http://localhost/"));
            //MockRequest.Setup(request => request.PathInfo).Returns(string.Empty);
            //MockRequest.Setup(request => request.ServerVariables).Returns(new NameValueCollection());

            return this;
        }
    }
}
