using System;
using BetterModules.Core.Models;
using NUnit.Framework;

namespace BetterModules.Core.Tests.Models
{
    [TestFixture]
    public class EquatableEntityTests
    {
        [Test]
        public void Should_Compare_Empty_Entities_Using_Equal_Operator()
        {
            var model1 = new TestEquatableEntity();
            var model2 = new TestEquatableEntity();
            var model3 = model1;

            Assert.IsTrue(model1 == model3);
            Assert.IsFalse(model1 == model2);
        }

        [Test]
        public void Should_Compare_Empty_Entities_Using_NotEqual_Operator()
        {
            var model1 = new TestEquatableEntity();
            var model2 = new TestEquatableEntity();
            var model3 = model1;

            Assert.IsFalse(model1 != model3);
            Assert.IsTrue(model1 != model2);
        }
        
        [Test]
        public void Should_Compare_Entities_Using_Equal_Operator()
        {
            var model1 = new TestEquatableEntity {Id = Guid.NewGuid()};
            var model2 = new TestEquatableEntity {Id = Guid.NewGuid()};
            var model3 = model1;

            Assert.IsTrue(model1 == model3);
            Assert.IsFalse(model1 == model2);
        }

        [Test]
        public void Should_Compare_Entities_Using_NotEqual_Operator()
        {
            var model1 = new TestEquatableEntity { Id = Guid.NewGuid() };
            var model2 = new TestEquatableEntity { Id = Guid.NewGuid() };
            var model3 = model1;

            Assert.IsFalse(model1 != model3);
            Assert.IsTrue(model1 != model2);
        }

        [Test]
        public void Should_Compare_Two_Entities_With_Same_Id()
        {
            var guid = Guid.NewGuid();
            var model1 = new TestEquatableEntity { Id = guid };
            var model2 = new TestEquatableEntity { Id = guid };
            
            Assert.AreEqual(model1, model2);
            Assert.IsTrue(model1 == model2);
            Assert.IsTrue(model1.Equals(model2));
            Assert.IsTrue(model2.Equals(model1));
        }
        
        [Test]
        public void Should_Compare_Two_Entities_With_Same_Id_HashCodes()
        {
            var guid = Guid.NewGuid();
            var model1 = new TestEquatableEntity { Id = guid };
            var model2 = new TestEquatableEntity { Id = guid };
            
            Assert.AreEqual(model1.GetHashCode(), model2.GetHashCode());
        }
        
        [Test]
        public void Should_Compare_Two_Entities_Without_Id_HashCodes()
        {
            var model1 = new TestEquatableEntity();
            var model2 = new TestEquatableEntity();
            
            Assert.AreNotEqual(model1.GetHashCode(), model2.GetHashCode());
        }

        private class TestEquatableEntity : EquatableEntity<TestEquatableEntity>
        {
        }
    }
}
