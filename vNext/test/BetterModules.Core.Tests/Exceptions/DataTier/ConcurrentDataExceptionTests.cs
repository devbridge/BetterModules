using System;
using BetterModules.Core.Exceptions.DataTier;
using BetterModules.Sample.Module.Models;
using NUnit.Framework;

namespace BetterModules.Core.Tests.Exceptions.DataTier
{
    [TestFixture]
    public class ConcurrentDataExceptionTests
    {
        private const string Message = "TestExcMessage";

        [Test]
        public void Should_Create_Exception_With_Message()
        {
            var exception = new ConcurrentDataException(Message);
            
            Assert.AreEqual(exception.Message, Message);
            Assert.IsNull(exception.InnerException);
            Assert.IsNull(exception.StaleEntity);
        }
        
        [Test]
        public void Should_Create_Exception_With_Message_And_InnerException()
        {
            var innerException = new Exception(Message);
            var exception = new ConcurrentDataException(Message, innerException);
            
            Assert.AreEqual(exception.Message, Message);
            Assert.AreEqual(exception.InnerException, innerException);
            Assert.IsNull(exception.StaleEntity);
        }
        
        [Test]
        public void Should_Create_Exception_With_Stale_Entity()
        {
            var guid = Guid.NewGuid();
            var entity = new TestItemModel { Id = guid };
            var exception = new ConcurrentDataException(entity);
            
            Assert.IsNotNull(exception.Message);
            Assert.IsTrue(exception.Message.Contains(guid.ToString()));
            Assert.IsTrue(exception.Message.Contains("TestItemModel"));
            Assert.AreEqual(exception.StaleEntity, entity);
            Assert.IsNull(exception.InnerException);
        }
    }
}
