using System;

namespace BetterModules.Core.Web.Environment.Application
{
    /// <summary>
    /// Application assembly pre-start attribute
    /// Based on: https://github.com/davidebbo/WebActivator
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public sealed class WebApplicationPreStartAttribute : BaseActivationMethodAttribute
    {
        public WebApplicationPreStartAttribute(Type type, string methodName)
            : base(type, methodName)
        {
        }

        // Set this to true to have the method run in designer mode (in addition to running at runtime)
        public bool RunInDesigner { get; set; }

        public override bool ShouldRunInDesignerMode()
        {
            return RunInDesigner;
        }
    }
}
