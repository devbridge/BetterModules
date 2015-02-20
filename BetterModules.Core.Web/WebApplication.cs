using BetterModules.Core.Web.Environment.Application;

namespace BetterModules.Core.Web
{
    public static class WebApplication
    {
        public static void Initialize()
        {
            WebApplicationEntryPoint.PreApplicationStart();
        }
    }
}