//using System;
//using System.IO;
//using System.Web;
//using BetterModules.Core.Web.Configuration;
//using BetterModules.Core.Web.Web;
//using Microsoft.AspNet.Http;
//using Microsoft.Framework.OptionsModel;
//using Moq;
//using Xunit;

//namespace BetterModules.Core.Web.Tests.Web
//{
//    public class DefaultHttpContextAccessorTests
//    {
//        private HttpContext currentContext;

//        [Fact]
//        public void Should_Return_Correct_Current_HttpContext()
//        {
//            CreateHttpContext();
//            var webConfiguration = new Mock<IOptions<DefaultWebConfigurationSection>>();
//            var accessor = new DefaultHttpContextAccessor(webConfiguration.Object);
//            var current = accessor.HttpContext;

//            Assert.NotNull(current);
//            Assert.True(current.Items.Contains("TestKey"));
//            Assert.Equal(current.Items["TestKey"], "TestValue");

//            RestoreContext();
//        }

//        [Fact]
//        public void Should_Map_Local_Path_WithoutContext_Correctly()
//        {
//            currentContext = HttpContext.Current;
//            HttpContext.Current = null;

//            var webConfiguration = new Mock<IWebConfiguration>();
//            var accessor = new DefaultHttpContextAccessor(webConfiguration.Object);

//            var path = accessor.MapPath("test\\test1");
//            Assert.NotNull(path);
//            Assert.True(path.EndsWith("test\\test1"));
//            Assert.True(path.StartsWith(AppDomain.CurrentDomain.BaseDirectory));

//            RestoreContext();
//        }

//        private void CreateHttpContext()
//        {
//            currentContext = HttpContext.Current;

//            var fakeContext = new HttpContext(new HttpRequest("c:\\test.dll", "http://localhost", null), new HttpResponse(new StringWriter()));
//            fakeContext.Items.Add("TestKey", "TestValue");
//            HttpContext.Current = fakeContext;
//        }

//        private void RestoreContext()
//        {
//            HttpContext.Current = currentContext;
//        }
//    }
//}
