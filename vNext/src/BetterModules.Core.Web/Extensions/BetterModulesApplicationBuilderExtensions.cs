using BetterModules.Core.Web.Web.EmbeddedResources;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.StaticFiles;
using Microsoft.Framework.DependencyInjection;

namespace BetterModules.Core.Web.Extensions
{
    public static class BetterModulesApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseBetterStaticFiles(this IApplicationBuilder app)
        {
            return app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = app.ApplicationServices.GetService<IEmbeddedResourceProvider>()
            });
        }
    }
}
