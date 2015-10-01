using System;
using BetterModules.Core.Models;
using Xunit;

namespace BetterModules.Core.Tests.Models
{
    public class EquatableEntityTests
    {
        [Fact]
        public void Should_Compare_Empty_Entities_Using_Equal_Operator()
        {
            var model1 = new TestEquatableEntity();
            var model2 = new TestEquatableEntity();
            var model3 = model1;

            Assert.True(model1 == model3);
            Assert.False(model1 == model2);
        }

        [Fact]
        public void Should_Compare_Empty_Entities_Using_NotEqual_Operator()
        {
            var model1 = new TestEquatableEntity();
            var model2 = new TestEquatableEntity();
            var model3 = model1;

            Assert.False(model1 != model3);
            Assert.True(model1 != model2);
        }
        
        [Fact]
        public void Should_Compare_Entities_Using_Equal_Operator()
        {
            var model1 = new TestEquatableEntity {Id = Guid.NewGuid()};
            var model2 = new TestEquatableEntity {Id = Guid.NewGuid()};
            var model3 = model1;

            Assert.True(model1 == model3);
            Assert.False(model1 == model2);
        }

        [Fact]
        public void Should_Compare_Entities_Using_NotEqual_Operator()
        {
            var model1 = new TestEquatableEntity { Id = Guid.NewGuid() };
            var model2 = new TestEquatableEntity { Id = Guid.NewGuid() };
            var model3 = model1;

            Assert.False(model1 != model3);
            Assert.True(model1 != model2);
        }

        [Fact]
        public void Should_Compare_Two_Entities_With_Same_Id()
        {
            var guid = Guid.NewGuid();
            var model1 = new TestEquatableEntity { Id = guid };
            var model2 = new TestEquatableEntity { Id = guid };
            
            Assert.Equal(model1, model2);
            Assert.True(model1 == model2);
            Assert.True(model1.Equals(model2));
            Assert.True(model2.Equals(model1));
        }
        
        [Fact]
        public void Should_Compare_Two_Entities_With_Same_Id_HashCodes()
        {
            var guid = Guid.NewGuid();
            var model1 = new TestEquatableEntity { Id = guid };
            var model2 = new TestEquatableEntity { Id = guid };
            
            Assert.Equal(model1.GetHashCode(), model2.GetHashCode());
        }
        
        [Fact]
        public void Should_Compare_Two_Entities_Without_Id_HashCodes()
        {
            var model1 = new TestEquatableEntity();
            var model2 = new TestEquatableEntity();
            
            Assert.NotEqual(model1.GetHashCode(), model2.GetHashCode());
        }

        private class TestEquatableEntity : EquatableEntity<TestEquatableEntity>
        {
        }
    }
}
