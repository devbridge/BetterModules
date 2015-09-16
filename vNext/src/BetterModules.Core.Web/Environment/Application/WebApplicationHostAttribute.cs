using System;

namespace BetterModules.Core.Web.Environment.Application
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
    public class WebApplicationHostAttribute : BaseAttribute
    {
        public WebApplicationHostAttribute(Type type) : base(type)
        {
        }
    }
}
