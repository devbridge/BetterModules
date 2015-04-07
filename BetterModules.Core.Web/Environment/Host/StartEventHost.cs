using System;
using System.Web;
using BetterModules.Core.Web.Environment.Application;
using BetterModules.Core.Web.Environment.Host;
using BetterModules.Events;

[assembly : WebApplicationHost(typeof(StartEventHost), Order = Int32.MaxValue)]
namespace BetterModules.Core.Web.Environment.Host
{
    public sealed class StartEventHost : DefaultWebApplicationAutoHost
    {
        public override void Init(HttpApplication context)
        {
            lock (Lock)
            {
                if (InitializedCount++ == 0)
                {
                    WebCoreEvents.Instance.OnHostStart(context);
                }
            }
        }

        public override void Dispose()
        {
            lock (Lock)
            {
                if (--InitializedCount == 0)
                WebCoreEvents.Instance.OnHostStop(Application);
            }
        }
    }
}
