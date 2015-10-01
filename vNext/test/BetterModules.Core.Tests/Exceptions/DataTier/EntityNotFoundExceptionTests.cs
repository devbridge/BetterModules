using System;
using BetterModules.Core.Exceptions.DataTier;
using BetterModules.Sample.Module.Models;
using NUnit.Framework;

namespace BetterModules.Core.Tests.Exceptions.DataTier
{
    [TestFixture]
    public class EntityNotFoundExceptionTests
    {
        private const string Message = "TestExcMessage";

        [Test]
        public void Should_Create_Exception_With_Message()
        {
            var exception = new EntityNotFoundException(Message);

            Assert.AreEqual(exception.Message, Message);
            Assert.IsNull(exception.InnerException);
        }

        [Test]
        public void Should_Create_Exception_With_Message_And_InnerException()
        {
            var innerException = new Exception(Message);
            var exception = new EntityNotFoundException(Message, innerException);

            Assert.AreEqual(exception.Message, Message);
            Assert.AreEqual(exception.InnerException, innerException);
        }

        [Test]
        public void Should_Create_Exception_With_Type_And_Id()
        {
            var guid = Guid.NewGuid();
            var exception = new EntityNotFoundException(typeof(TestItemModel), guid);

            Assert.IsNotNull(exception.Message);
            Assert.IsTrue(exception.Message.Contains(guid.ToString()));
            Assert.IsTrue(exception.Message.Contains("TestItemModel"));
            Assert.IsNull(exception.InnerException);
        }
        
        [Test]
        public void Should_Create_Exception_With_Type_And_Filter()
        {
            var filter = "test filter";
            var exception = new EntityNotFoundException(typeof(TestItemModel), filter);

            Assert.IsNotNull(exception.Message);
            Assert.IsTrue(exception.Message.Contains(filter));
            Assert.IsTrue(exception.Message.Contains("TestItemModel"));
            Assert.IsNull(exception.InnerException);
        }
    }
}
