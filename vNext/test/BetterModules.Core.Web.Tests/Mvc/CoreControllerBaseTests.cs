//using System.Linq;
//using BetterModules.Core.Infrastructure.Commands;
//using BetterModules.Core.Web.Models;
//using BetterModules.Core.Web.Mvc;
//using BetterModules.Sample.Web.Module.Controllers;
//using Microsoft.AspNet.Mvc;
//using Xunit;

//namespace BetterModules.Core.Web.Tests.Mvc
//{
//    public class CoreControllerBaseTests
//    {
//        [Fact]
//        public void Should_Return_Correct_Controller_Messages()
//        {
//            using (var container = ContextScopeProvider.CreateChildContainer())
//            {
//                var controller = container.Resolve<SampleWebController>() as ICommandContext;

//                Assert.NotNull(controller);
//                Assert.NotNull(controller.Messages);
//            }
//        }

//        [Fact]
//        public void Should_Create_Success_Json_And_Append_Messages()
//        {
//            using (var container = ContextScopeProvider.CreateChildContainer())
//            {
//                var controller = PrepareController(container);
//                var messages = new[] { "Message 1" };
//                var json = new WireJson { Success = true, Messages = messages.ToArray() };
//                var jsonResult = controller.Json(json);

//                Assert.Equal(jsonResult.Data, json);
//                AssertJson(messages, jsonResult);
//            }
//        }
        
//        [Fact]
//        public void Should_Create_Failed_Json_And_Append_Messages()
//        {
//            using (var container = ContextScopeProvider.CreateChildContainer())
//            {
//                var controller = PrepareController(container);
//                var messages = new[] { "Message 1" };
//                var json = new WireJson { Success = false, Messages = messages.ToArray() };
//                var jsonResult = controller.Json(json);

//                Assert.Equal(jsonResult.Data, json);
//                AssertJson(messages, jsonResult);
//            }
//        }
        
//        //[Fact]
//        public void Should_Create_Success_WireJson_And_Append_Messages()
//        {
//            using (var container = ContextScopeProvider.CreateChildContainer())
//            {
//                var controller = PrepareController(container);
//                var data = new { Test = "Test" };
//                var jsonResult = controller.WireJson(true, data);

//                var result = AssertJson(null, jsonResult);
//                Assert.Equal(result.Data, data);
//            }
//        }
        
//        [Fact]
//        public void Should_Create_Success_ComboWireJson_And_Append_Messages()
//        {
//            using (var container = ContextScopeProvider.CreateChildContainer())
//            {
//                var controller = PrepareController(container);
//                var data = new { Test = "Test" };
//                const string html = "html";
//                var jsonResult = controller.ComboWireJson(true, html, data);

//                var result = AssertJson(null, jsonResult);
//                Assert.Equal(result.Data, data);

//                Assert.True(result is ComboWireJson);
//                var comboResult = (ComboWireJson)result;

//                Assert.Equal(comboResult.Data, data);
//                Assert.Equal(comboResult.Html, html);
//            }
//        }

//        private CoreControllerBase PrepareController(ILifetimeScope container)
//        {
//            var controller = container.Resolve<SampleWebController>();
//            Assert.NotNull(controller);

//            controller.Messages.AddError("Test Error");
//            controller.Messages.AddSuccess("Test Success");

//            return controller;
//        }

//        private WireJson AssertJson(string[] messages, JsonResult jsonResult)
//        {
//            Assert.NotNull(jsonResult);

//            Assert.True(jsonResult.Data is WireJson);
//            var returnedJson = (WireJson)jsonResult.Data;

//            var count = messages != null ? messages.Count() + 1 : 1;
//            Assert.Equal(returnedJson.Messages.Count(), count);
//            if (returnedJson.Success)
//            {
//                Assert.True(returnedJson.Messages.Contains("Test Success"));
//            }
//            else
//            {
//                Assert.True(returnedJson.Messages.Contains("Test Error"));
//            }

//            if (messages != null)
//            {
//                foreach (var message in messages)
//                {
//                    Assert.True(returnedJson.Messages.Contains(message));
//                }
//            }

//            return returnedJson;
//        }
//    }
//}
