using System;
using System.Text;
using BetterModules.Sample.Module.Models;

namespace BetterModules.Core.Tests.TestHelpers
{
    public class RandomTestDataProvider
    {
        private readonly Random random = new Random();

        public string ProvideRandomString(int length = 100)
        {
            var sb = new StringBuilder();
            while (sb.Length < length)
            {
                sb.Append(Guid.NewGuid().ToString().Replace("-", string.Empty));
            }
            return sb.ToString().Substring(0, length);
        }

        public int ProvideRandomNumber(int min = 1, int max = 1000)
        {
            return random.Next(min, max);
        }

        public decimal ProvideRandomNumber(decimal min, decimal max, int scale)
        {
            return Math.Round((decimal)((double)min + (random.NextDouble() * ((double)max - (double)min))), scale);
        }

        public decimal ProvideRandomNumber(decimal min, decimal max)
        {
            return (decimal)((double)min + (random.NextDouble() * ((double)max - (double)min)));
        }

        public double ProvideRandomNumber(double min, double max)
        {
            return min + (random.NextDouble() * (max - min));
        }

        public DateTime ProvideRandomDateTime()
        {
            return new DateTime(ProvideRandomNumber(1990, 2019), ProvideRandomNumber(1, 12), ProvideRandomNumber(1, 29),
                                ProvideRandomNumber(0, 23), ProvideRandomNumber(0, 59), ProvideRandomNumber(0, 59));
        }

        public TestItemModel ProvideRandomTestItemModel(TestItemCategory category = null)
        {
            var model = new TestItemModel();
            model.Name = ProvideRandomString();

            model.Category = category ?? ProvideRandomTestItemCategory();

            return model;
        }

        public TestItemCategory ProvideRandomTestItemCategory()
        {
            var model = new TestItemCategory();
            model.Name = ProvideRandomString();
            
            return model;
        }
    }
}
