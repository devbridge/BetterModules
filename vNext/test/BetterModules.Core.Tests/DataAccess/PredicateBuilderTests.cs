using System.Linq;
using BetterModules.Core.DataAccess;
using Xunit;

namespace BetterModules.Core.Tests.DataAccess
{
    public class PredicateBuilderTests
    {
        private static Fruit[] Values =
        {
            new Fruit {Name ="orange", IsCitrus = true},
            new Fruit {Name ="lemon", IsCitrus = true},
            new Fruit {Name ="pear", IsCitrus = false},
            new Fruit {Name ="apple", IsCitrus = false}
        };

        [Fact]
        public void Should_Filter_False_Or_Correctly()
        {
            var predicateBuilder = PredicateBuilder.False<Fruit>();
            predicateBuilder = predicateBuilder.Or(p => p.Name == "orange");
            predicateBuilder = predicateBuilder.Or(p => p.Name == "apple");

            var result = Values.AsQueryable().Where(predicateBuilder).ToList();
            Assert.NotNull(result);
            Assert.Equal(result.Count, 2);
            Assert.Contains(Values.First(v => v.Name == "orange"), result);
            Assert.Contains(Values.First(v => v.Name == "apple"), result);
        }
        
        [Fact]
        public void Should_Filter_True_And_Correctly()
        {
            var predicateBuilder = PredicateBuilder.True<Fruit>();
            predicateBuilder = predicateBuilder.And(p => p.Name == "lemon");
            predicateBuilder = predicateBuilder.And(p => p.IsCitrus);

            var result = Values.AsQueryable().Where(predicateBuilder).ToList();
            Assert.NotNull(result);
            Assert.Equal(result.Count, 1);
            Assert.Contains(Values.First(v => v.Name == "lemon"), result);
        }

        private class Fruit
        {
            public string Name { get; set; }
            
            public bool IsCitrus { get; set; }
        }
    }
}
