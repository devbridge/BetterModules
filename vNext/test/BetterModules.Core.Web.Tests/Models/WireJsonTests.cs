using System.Linq;
using BetterModules.Core.Web.Models;
using Xunit;

namespace BetterModules.Core.Web.Tests.Models
{
    public class WireJsonTests
    {
        [Fact]
        public void Should_Initialize_Empty_Constructor()
        {
            var json = new WireJson();

            Assert.False(json.Success);
            Assert.Equal(json.DataType, "html");
            Assert.Null(json.Data);
            Assert.Null(json.Messages);
        }
        
        [Fact]
        public void Should_Initialize_Success()
        {
            var json = new WireJson(true);

            Assert.True(json.Success);
            Assert.Equal(json.DataType, "html");
            Assert.Null(json.Data);
            Assert.Empty(json.Messages);
        }
        
        [Fact]
        public void Should_Initialize_Success_And_Messages()
        {
            const string message1 = "Message1";
            const string message2 = "Message2";

            var json = new WireJson(true, new[] { message1, message2 });

            Assert.True(json.Success);
            Assert.Equal(json.DataType, "html");
            Assert.Null(json.Data);
            Assert.NotNull(json.Messages);
            Assert.Equal(json.Messages.Length, 2);
            Assert.True(json.Messages.Any(m => m == message1));
            Assert.True(json.Messages.Any(m => m == message2));
        }
        
        [Fact]
        public void Should_Initialize_Success_Html_And_Messages()
        {
            const string html = "html";
            const string message1 = "Message1";
            const string message2 = "Message2";

            var json = new WireJson(true, html, message1, message2);

            Assert.True(json.Success);
            Assert.Equal(json.DataType, "html");
            Assert.Equal(json.Data, html);
            Assert.NotNull(json.Messages);
            Assert.Equal(json.Messages.Length, 2);
            Assert.True(json.Messages.Any(m => m == message1));
            Assert.True(json.Messages.Any(m => m == message2));
        }
        
        [Fact]
        public void Should_Initialize_Success_DataType_And_Messages()
        {
            const string data = "html";
            const string dataType = "test-type";
            const string message1 = "Message1";
            const string message2 = "Message2";

            var json = new WireJson(true, dataType, data, new[] { message1, message2 });

            Assert.True(json.Success);
            Assert.Equal(json.DataType, dataType);
            Assert.Equal(json.Data, data);
            Assert.NotNull(json.Messages);
            Assert.Equal(json.Messages.Length, 2);
            Assert.True(json.Messages.Any(m => m == message1));
            Assert.True(json.Messages.Any(m => m == message2));
        }

        [Fact]
        public void Should_Initialize_Success_And_Object()
        {
            dynamic data = new { Test = 1 };

            var json = new WireJson(true, data);

            Assert.True(json.Success);
            Assert.Null(json.DataType);
            Assert.Equal(json.Data, data);
            Assert.Null(json.Messages);
        }
    }
}
