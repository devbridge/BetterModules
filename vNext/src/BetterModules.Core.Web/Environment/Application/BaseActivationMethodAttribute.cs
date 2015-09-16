using System;
using System.Reflection;

namespace BetterModules.Core.Web.Environment.Application
{
    /// <summary>
    /// Base class of all the activation attributes
    /// Based on: https://github.com/davidebbo/WebActivator
    /// </summary>
    
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public abstract class BaseActivationMethodAttribute : BaseAttribute
    {
        private string _methodName;

        public BaseActivationMethodAttribute(Type type, string methodName) : base(type)
        {
            _methodName = methodName;
        }

        public string MethodName
        {
            get
            {
                return _methodName;
            }
        }

        public void InvokeMethod()
        {
            // Get the method
            MethodInfo method = Type.GetMethod(
                MethodName,
                BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);

            if (method == null)
            {
                throw new ArgumentException(
                    String.Format("The type {0} doesn't have a static method named {1}",
                        Type, MethodName));
            }

            // Invoke it
            method.Invoke(null, null);
        }

        public virtual bool ShouldRunInDesignerMode()
        {
            return false;
        }
    }
}
