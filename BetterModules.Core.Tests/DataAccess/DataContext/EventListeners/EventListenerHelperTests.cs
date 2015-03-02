using System;

using BetterModules.Core.DataAccess.DataContext.EventListeners;
using BetterModules.Core.Security;
using BetterModules.Sample.Module.Models;

using Moq;

using NUnit.Framework;

namespace BetterModules.Core.Tests.DataAccess.DataContext.EventListeners
{
    [TestFixture]
    public class EventListenerHelperTests : TestBase
    {
        [Test]
        public void Should_Update_Creation_Fields_Correctly()
        {
            var principalProvider = new Mock<IPrincipalProvider>();
            principalProvider.Setup(p => p.CurrentPrincipalName).Returns("TestPrincipal");
            var helper = new EventListenerHelper(principalProvider.Object);

            var entity = CreateEntity();
            helper.OnCreate(entity);

            EnsureDeletionPropertiesUntouched(entity);

            Assert.IsTrue(entity.CreatedOn != DateTime.MinValue);
            Assert.IsTrue(entity.ModifiedOn != DateTime.MinValue);
            Assert.AreEqual(entity.CreatedByUser, "TestPrincipal");
            Assert.AreEqual(entity.ModifiedByUser, "TestPrincipal");
        }
        
        [Test]
        public void Should_Update_Modification_Fields_Correctly()
        {
            var principalProvider = new Mock<IPrincipalProvider>();
            principalProvider.Setup(p => p.CurrentPrincipalName).Returns("TestPrincipal");
            var helper = new EventListenerHelper(principalProvider.Object);

            var entity = CreateEntity();
            helper.OnModify(entity);

            EnsureDeletionPropertiesUntouched(entity);
            EnsureCreationPropertiesUntouched(entity);

            Assert.IsTrue(entity.ModifiedOn != DateTime.MinValue);
            Assert.AreEqual(entity.ModifiedByUser, "TestPrincipal");
        }
        
        [Test]
        public void Should_Update_Deletion_Fields_Correctly()
        {
            var principalProvider = new Mock<IPrincipalProvider>();
            principalProvider.Setup(p => p.CurrentPrincipalName).Returns("TestPrincipal");
            var helper = new EventListenerHelper(principalProvider.Object);

            var entity = CreateEntity();
            helper.OnDelete(entity);

            EnsureCreationPropertiesUntouched(entity);
            EnsureModificationPropertiesUntouched(entity);

            Assert.IsTrue(entity.IsDeleted);
            Assert.IsTrue(entity.DeletedOn != DateTime.MinValue);
            Assert.AreEqual(entity.DeletedByUser, "TestPrincipal");
        }

        private void EnsureCreationPropertiesUntouched(TestItemModel entity)
        {
            Assert.AreEqual(entity.CreatedOn, DateTime.MinValue);
            Assert.IsNull(entity.CreatedByUser);
        }
        
        private void EnsureDeletionPropertiesUntouched(TestItemModel entity)
        {
            Assert.IsNull(entity.DeletedByUser);
            Assert.IsNull(entity.DeletedOn);
            Assert.IsFalse(entity.IsDeleted);
        }
        
        private void EnsureModificationPropertiesUntouched(TestItemModel entity)
        {
            Assert.AreEqual(entity.ModifiedOn, DateTime.MinValue);
            Assert.IsNull(entity.ModifiedByUser);
        }

        private TestItemModel CreateEntity()
        {
            var entity =TestDataProvider.ProvideRandomTestItemModel();
            entity.CreatedByUser = entity.ModifiedByUser = entity.DeletedByUser = null;
            entity.CreatedOn = entity.ModifiedOn = DateTime.MinValue;
            entity.DeletedOn = null;
            entity.IsDeleted = false;

            return entity;
        }
    }
}
