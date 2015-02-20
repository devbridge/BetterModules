using BetterModules.Core.Events;
using NUnit.Framework;

namespace BetterModules.Core.Tests.Events
{
    [TestFixture]
    public class SingleItemEventArgsTests : TestBase
    {
        [Test]
        public void Should_Assign_Correct_Single_Event_Arg()
        {
            var args = new SingleItemEventArgs<int>(13);

            Assert.AreEqual(args.Item, 13);
        }
    }
}
