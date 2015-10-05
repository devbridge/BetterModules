using System;
using System.Collections.Generic;
using System.Linq;
using BetterModules.Core.Web.Models;
using Xunit;

namespace BetterModules.Core.Web.Tests.Models
{
    public class UserMessagesTests
    {
        [Fact]
        public void Should_Create_Empty_Warn_Messages()
        {
            var messages = new UserMessages();

            Assert.NotNull(messages.Error);
            Assert.NotNull(messages.Success);
            Assert.NotNull(messages.Warn);
            Assert.NotNull(messages.Info);

            Assert.Empty(messages.Error);
            Assert.Empty(messages.Success);
            Assert.Empty(messages.Warn);
            Assert.Empty(messages.Info);
        }
        
        [Fact]
        public void Should_Add_Return_Messages_Correctly()
        {
            var messages = new UserMessages();

            Assert.NotNull(messages.Error);
            Assert.NotNull(messages.Success);
            Assert.NotNull(messages.Warn);
            Assert.NotNull(messages.Info);

            messages.AddError("E1");
            messages.AddError("E2");
            messages.AddWarn("W1");
            messages.AddWarn("W2");
            messages.AddSuccess("S1");
            messages.AddSuccess("S2");
            messages.AddInfo("I1");
            messages.AddInfo("I2");

            Assert.Equal(messages.Error.Count, 2);
            Assert.True(messages.Error.Any(e => e == "E1"));
            Assert.True(messages.Error.Any(e => e == "E2"));
            
            Assert.Equal(messages.Warn.Count, 2);
            Assert.True(messages.Warn.Any(e => e == "W1"));
            Assert.True(messages.Warn.Any(e => e == "W2"));
            
            Assert.Equal(messages.Success.Count, 2);
            Assert.True(messages.Success.Any(e => e == "S1"));
            Assert.True(messages.Success.Any(e => e == "S2"));
            
            Assert.Equal(messages.Info.Count, 2);
            Assert.True(messages.Info.Any(e => e == "I1"));
            Assert.True(messages.Info.Any(e => e == "I2"));
        }
        
        [Fact]
        public void Should_Clear_Messages_Correctly()
        {
            var messages = new UserMessages();

            Assert.NotNull(messages.Error);
            Assert.NotNull(messages.Success);
            Assert.NotNull(messages.Warn);
            Assert.NotNull(messages.Info);

            messages.AddError("M");
            messages.AddSuccess("M");
            messages.AddInfo("M");
            messages.AddWarn("M");
            
            Assert.Equal(messages.Error.Count, 1);
            Assert.Equal(messages.Info.Count, 1);
            Assert.Equal(messages.Warn.Count, 1);
            Assert.Equal(messages.Success.Count, 1);
            
            messages.Clear();

            Assert.Empty(messages.Error);
            Assert.Empty(messages.Success);
            Assert.Empty(messages.Warn);
            Assert.Empty(messages.Info);
        }

        /// <summary>
        /// Should not to allow edit the collection (add new item)
        /// </summary>
        [Fact]
        public void Should_Throw_ReadOnly_Exception()
        {
            var thrown = 0;

            var messages = new UserMessages();

            new List<Action>
                {
                    () => messages.Info.Add("M"),
                    () => messages.Warn.Add("M"),
                    () => messages.Success.Add("M"),
                    () => messages.Error.Add("M")
                }.ForEach(
                action =>
                {
                    try
                    {
                        action();
                    }
                    catch (NotSupportedException)
                    {
                        thrown++;
                    }
                });

            Assert.Equal(thrown, 4);
        }
    }
}
