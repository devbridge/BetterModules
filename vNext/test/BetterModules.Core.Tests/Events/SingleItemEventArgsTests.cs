using BetterModules.Events;
using Xunit;

namespace BetterModules.Core.Tests.Events
{
    public class SingleItemEventArgsTests
    {
        [Fact]
        public void Should_Assign_Correct_Single_Event_Arg()
        {
            var args = new SingleItemEventArgs<int>(13);

            Assert.Equal(args.Item, 13);
        }
    }
}
