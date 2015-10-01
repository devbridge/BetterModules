using System;
using BetterModules.Core.Exceptions.DataTier;
using BetterModules.Sample.Module.Models;
using Xunit;

namespace BetterModules.Core.Tests.Exceptions.DataTier
{
    public class ConcurrentDataExceptionTests
    {
        private const string Message = "TestExcMessage";

        [Fact]
        public void Should_Create_Exception_With_Message()
        {
            var exception = new ConcurrentDataException(Message);
            
            Assert.Equal(exception.Message, Message);
            Assert.Null(exception.InnerException);
            Assert.Null(exception.StaleEntity);
        }
        
        [Fact]
        public void Should_Create_Exception_With_Message_And_InnerException()
        {
            var innerException = new Exception(Message);
            var exception = new ConcurrentDataException(Message, innerException);
            
            Assert.Equal(exception.Message, Message);
            Assert.Equal(exception.InnerException, innerException);
            Assert.Null(exception.StaleEntity);
        }
        
        [Fact]
        public void Should_Create_Exception_With_Stale_Entity()
        {
            var guid = Guid.NewGuid();
            var entity = new TestItemModel { Id = guid };
            var exception = new ConcurrentDataException(entity);
            
            Assert.NotNull(exception.Message);
            Assert.True(exception.Message.Contains(guid.ToString()));
            Assert.True(exception.Message.Contains("TestItemModel"));
            Assert.Equal(exception.StaleEntity, entity);
            Assert.Null(exception.InnerException);
        }
    }
}
