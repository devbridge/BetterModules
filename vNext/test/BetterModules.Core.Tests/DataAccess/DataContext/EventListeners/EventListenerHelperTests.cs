using System;

using BetterModules.Core.DataAccess.DataContext.EventListeners;
using BetterModules.Core.Security;
using BetterModules.Sample.Module.Models;

using Moq;
using Xunit;


namespace BetterModules.Core.Tests.DataAccess.DataContext.EventListeners
{
    public class EventListenerHelperTests
    {
        [Fact]
        public void Should_Update_Creation_Fields_Correctly()
        {
            var principalProvider = new Mock<IPrincipalProvider>();
            principalProvider.Setup(p => p.CurrentPrincipalName).Returns("TestPrincipal");
            var helper = new EventListenerHelper(principalProvider.Object);

            var entity = CreateEntity();
            helper.OnCreate(entity);

            EnsureDeletionPropertiesUntouched(entity);

            Assert.True(entity.CreatedOn != DateTime.MinValue);
            Assert.True(entity.ModifiedOn != DateTime.MinValue);
            Assert.Equal(entity.CreatedByUser, "TestPrincipal");
            Assert.Equal(entity.ModifiedByUser, "TestPrincipal");
        }
        
        [Fact]
        public void Should_Update_Modification_Fields_Correctly()
        {
            var principalProvider = new Mock<IPrincipalProvider>();
            principalProvider.Setup(p => p.CurrentPrincipalName).Returns("TestPrincipal");
            var helper = new EventListenerHelper(principalProvider.Object);

            var entity = CreateEntity();
            helper.OnModify(entity);

            EnsureDeletionPropertiesUntouched(entity);
            EnsureCreationPropertiesUntouched(entity);

            Assert.True(entity.ModifiedOn != DateTime.MinValue);
            Assert.Equal(entity.ModifiedByUser, "TestPrincipal");
        }
        
        [Fact]
        public void Should_Update_Deletion_Fields_Correctly()
        {
            var principalProvider = new Mock<IPrincipalProvider>();
            principalProvider.Setup(p => p.CurrentPrincipalName).Returns("TestPrincipal");
            var helper = new EventListenerHelper(principalProvider.Object);

            var entity = CreateEntity();
            helper.OnDelete(entity);

            EnsureCreationPropertiesUntouched(entity);
            EnsureModificationPropertiesUntouched(entity);

            Assert.True(entity.IsDeleted);
            Assert.True(entity.DeletedOn != DateTime.MinValue);
            Assert.Equal(entity.DeletedByUser, "TestPrincipal");
        }

        private void EnsureCreationPropertiesUntouched(TestItemModel entity)
        {
            Assert.Equal(entity.CreatedOn, DateTime.MinValue);
            Assert.Null(entity.CreatedByUser);
        }
        
        private void EnsureDeletionPropertiesUntouched(TestItemModel entity)
        {
            Assert.Null(entity.DeletedByUser);
            Assert.Null(entity.DeletedOn);
            Assert.False(entity.IsDeleted);
        }
        
        private void EnsureModificationPropertiesUntouched(TestItemModel entity)
        {
            Assert.Equal(entity.ModifiedOn, DateTime.MinValue);
            Assert.Null(entity.ModifiedByUser);
        }

        private TestItemModel CreateEntity()
        {
            var entity = new TestItemModel();
            entity.CreatedByUser = entity.ModifiedByUser = entity.DeletedByUser = null;
            entity.CreatedOn = entity.ModifiedOn = DateTime.MinValue;
            entity.DeletedOn = null;
            entity.IsDeleted = false;

            return entity;
        }
    }
}
