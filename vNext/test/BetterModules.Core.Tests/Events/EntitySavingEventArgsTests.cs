using BetterModules.Core.DataContracts;
using BetterModules.Events;
using Moq;
using NHibernate;
using Xunit;

namespace BetterModules.Core.Tests.Events
{
    public class EntitySavingEventArgsTests
    {
        [Fact]
        public void Should_Assign_Correct_Session_Arg()
        {
            var session = new Mock<ISession>().Object;
            var entity = new Mock<IEntity>().Object;

            var args = new EntitySavingEventArgs(entity, session);

            Assert.Equal(args.Session, session);
            Assert.Equal(args.Entity, entity);
        }
    }
}
