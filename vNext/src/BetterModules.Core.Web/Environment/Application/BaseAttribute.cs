using System;

namespace BetterModules.Core.Web.Environment.Application
{
    [AttributeUsage(AttributeTargets.Assembly)]
    public abstract class BaseAttribute : Attribute
    {
        private Type _type;
        public Type Type
        {
            get
            {
                return _type;
            }
        }

        public int Order { get; set; }
        public BaseAttribute(Type type)
        {
            _type = type;
        }
    }
}
