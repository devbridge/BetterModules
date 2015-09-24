using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BetterModules.Core.Web.Environment.Application;

namespace BetterModules.Core.Web.Extensions
{
    static class AssemblyExtensions
    {
        public static IEnumerable<T> GetAttributes<T>(this Assembly assembly)
            where T : BaseAttribute
        {
            try
            {
                return assembly.GetCustomAttributes(
                    typeof (T),
                    inherit: false).OfType<T>();
            }
            catch
            {
                return Enumerable.Empty<T>();
            }
        }
    }
}
