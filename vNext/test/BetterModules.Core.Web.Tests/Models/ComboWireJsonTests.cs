using System.Linq;
using BetterModules.Core.Web.Models;
using Xunit;

namespace BetterModules.Core.Web.Tests.Models
{
    public class ComboWireJsonTests
    {
        [Fact]
        public void Should_Initialize_Properties_Correctly()
        {
            dynamic data = new { Test = 1 };
            const string html = "test";
            const string message1 = "Message1";
            const string message2 = "Message2";

            var json = new ComboWireJson(true, html, data, message1, message2);

            Assert.Equal(json.Success, true);
            Assert.Equal(json.Html, html);
            Assert.Equal(json.Data, data);
            Assert.NotNull(json.Messages);
            Assert.Equal(json.Messages.Length, 2);
            Assert.True(json.Messages.Any(m => m == message1));
            Assert.True(json.Messages.Any(m => m == message2));
        }
    }
}
