using System;

namespace BetterModules.Sample.Module.Models
{
    public class SampleCalculator: ICalculator
    {
        public int Add(int x, int y)
        {
            return x + y;
        }

        public Tuple<double, double> FindRoots(double a, double b, double c)
        {
            var discriminant = b*b - 4*a*c;
            if (discriminant < 0)
            {
                return new Tuple<double, double>(0, 0);
            }
            var x1 = (-1*b + Math.Sqrt(discriminant))/2*a;
            var x2 = (-1*b - Math.Sqrt(discriminant))/2*a;

            return new Tuple<double, double>(x1, x2);
        }
    }
}