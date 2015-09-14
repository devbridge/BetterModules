using System;

namespace BetterModules.Sample.Module.Models
{
    public interface ICalculator
    {
        int Add(int x, int y);
        Tuple<double, double> FindRoots(double a, double b, double c);
    }
}