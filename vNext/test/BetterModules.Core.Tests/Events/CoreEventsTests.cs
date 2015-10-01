using BetterModules.Core.DataContracts;
using BetterModules.Events;
using Moq;
using NHibernate;
using Xunit;

namespace BetterModules.Core.Tests.Events
{
    public class CoreEventsTests
    {
        private int firedDelete;
        private int firedSave;
        private IEntity entity;
        private ISession session;

        [Fact]
        public void Should_FireDeleteEvents_Correctly()
        {
            firedDelete = 0;
            entity = new Mock<IEntity>().Object;
            
            CoreEvents.Instance.EntityDeleting += Instance_EntityDeleting;

            Assert.Equal(firedDelete, 0);
            CoreEvents.Instance.OnEntityDelete(entity);
            Assert.Equal(firedDelete, 1);
            CoreEvents.Instance.OnEntityDelete(entity);
            Assert.Equal(firedDelete, 2);

            CoreEvents.Instance.EntityDeleting -= Instance_EntityDeleting;
        }
        
        [Fact]
        public void Should_FireSaveEvents_Correctly()
        {
            firedSave = 0;
            entity = new Mock<IEntity>().Object;
            session = new Mock<ISession>().Object;

            CoreEvents.Instance.EntitySaving += Instance_EntitySaving;

            Assert.Equal(firedSave, 0);
            CoreEvents.Instance.OnEntitySaving(entity, session);
            Assert.Equal(firedSave, 1);
            CoreEvents.Instance.OnEntitySaving(entity, session);
            Assert.Equal(firedSave, 2);

            CoreEvents.Instance.EntitySaving -= Instance_EntitySaving;
        }

        void Instance_EntitySaving(EntitySavingEventArgs args)
        {
            firedSave ++;
        }

        void Instance_EntityDeleting(SingleItemEventArgs<IEntity> args)
        {
            firedDelete ++;
        }
    }
}
