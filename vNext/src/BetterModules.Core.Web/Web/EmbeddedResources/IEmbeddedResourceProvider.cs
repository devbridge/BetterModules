using System.Collections.Generic;
using System.Reflection;
using BetterModules.Core.Modules.Registration;
using Microsoft.AspNet.FileProviders;

namespace BetterModules.Core.Web.Web.EmbeddedResources
{
    public interface IEmbeddedResourceProvider: IFileProvider
    {
        void LoadEmbeddedResourcesFrom(ModuleRegistrationContext module);
        void LoadEmbeddedResourcesFrom(IEnumerable<ModuleRegistrationContext> modules);
    }
}