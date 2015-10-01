using BetterModules.Core.Extensions;
using Xunit;

namespace BetterModules.Core.Tests.Extensions
{
    public class TypeExtensionsTests
    {
        [Fact]
        public void Should_Check_GenericType_Correctly()
        {
            var instance = new TestClass();
            var type = instance.GetType();

            Assert.True(type.IsAssignableToGenericType(typeof(ITestInterface1<>)));
            Assert.True(type.IsAssignableToGenericType(typeof(ITestInterface2<,>)));
            Assert.True(type.IsAssignableToGenericType(typeof(AbstractTestBase<>)));
            Assert.True(type.IsAssignableToGenericType(typeof(AbstractTestBase<>)));
        }
        
        [Fact]
        public void Should_Check_NonGenericType_Correctly()
        {
            var instance = new TestClass2();
            var type = instance.GetType();

            Assert.False(type.IsAssignableToGenericType(typeof(ITestInterface1<>)));
            Assert.False(type.IsAssignableToGenericType(typeof(ITestInterface2<,>)));
            Assert.False(type.IsAssignableToGenericType(typeof(AbstractTestBase<>)));
            Assert.False(type.IsAssignableToGenericType(typeof(AbstractTestBase<>)));
        }

        public interface ITestInterface1<T>
        {
        }
        
        public interface ITestInterface2<T, T1> : ITestInterface1<T>
        {
        }

        public abstract class AbstractTestBase<T> : ITestInterface2<T, int>
        {
        }

        public class TestClass : AbstractTestBase<TestClass2>
        {
        }

        public class TestClass2
        {
        }
    }
}
