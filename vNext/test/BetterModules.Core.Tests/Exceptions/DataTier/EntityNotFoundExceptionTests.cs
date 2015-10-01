using System;
using BetterModules.Core.Exceptions.DataTier;
using BetterModules.Sample.Module.Models;
using Xunit;

namespace BetterModules.Core.Tests.Exceptions.DataTier
{
    public class EntityNotFoundExceptionTests
    {
        private const string Message = "TestExcMessage";

        [Fact]
        public void Should_Create_Exception_With_Message()
        {
            var exception = new EntityNotFoundException(Message);

            Assert.Equal(exception.Message, Message);
            Assert.Null(exception.InnerException);
        }

        [Fact]
        public void Should_Create_Exception_With_Message_And_InnerException()
        {
            var innerException = new Exception(Message);
            var exception = new EntityNotFoundException(Message, innerException);

            Assert.Equal(exception.Message, Message);
            Assert.Equal(exception.InnerException, innerException);
        }

        [Fact]
        public void Should_Create_Exception_With_Type_And_Id()
        {
            var guid = Guid.NewGuid();
            var exception = new EntityNotFoundException(typeof(TestItemModel), guid);

            Assert.NotNull(exception.Message);
            Assert.True(exception.Message.Contains(guid.ToString()));
            Assert.True(exception.Message.Contains("TestItemModel"));
            Assert.Null(exception.InnerException);
        }
        
        [Fact]
        public void Should_Create_Exception_With_Type_And_Filter()
        {
            var filter = "test filter";
            var exception = new EntityNotFoundException(typeof(TestItemModel), filter);

            Assert.NotNull(exception.Message);
            Assert.True(exception.Message.Contains(filter));
            Assert.True(exception.Message.Contains("TestItemModel"));
            Assert.Null(exception.InnerException);
        }
    }
}
