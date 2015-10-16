using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using BetterModules.Core.Modules.Registration;
using BetterModules.Core.Web.Modules;
using Microsoft.AspNet.FileProviders;
using Microsoft.Dnx.Runtime;
using Microsoft.Framework.Primitives;

namespace BetterModules.Core.Web.Web.EmbeddedResources
{
    public class DefaultEmbeddedResourceProvider: IEmbeddedResourceProvider
    {
        private readonly IFileProvider physicalFileProvider;
        private readonly ConcurrentDictionary<string, IFileInfo> embeddedFileInfoCache;
        private readonly DateTimeOffset lastModified = DateTimeOffset.MaxValue;

        public DefaultEmbeddedResourceProvider(IApplicationEnvironment appEnv)
        {
            physicalFileProvider = new PhysicalFileProvider(appEnv.ApplicationBasePath);
            embeddedFileInfoCache = new ConcurrentDictionary<string, IFileInfo>(StringComparer.Ordinal);
        }

        public IFileInfo GetFileInfo(string subpath)
        {
            // Check in physical File System whether the file is overriden  
            IFileInfo fileInfo = physicalFileProvider?.GetFileInfo(subpath);
            if (fileInfo != null && fileInfo.Exists)
            {
                return fileInfo;
            }

            if (subpath.StartsWith("/"))
            {
                subpath = subpath.Substring(1);
            }

            // Check for the file in embedded resources cache
            return embeddedFileInfoCache.TryGetValue(subpath, out fileInfo) ? fileInfo : new NotFoundFileInfo(subpath);
        }

        public void LoadEmbeddedResourcesFrom(ModuleRegistrationContext module)
        {
            LoadEmbeddedResourcesFrom(new List<ModuleRegistrationContext> { module });
        }

        public void LoadEmbeddedResourcesFrom(IEnumerable<ModuleRegistrationContext> modules)
        {
            foreach (var module in modules)
            {
                var descriptor = module.ModuleDescriptor as WebModuleDescriptor;
                if (descriptor != null)
                {
                    var assembly = Assembly.Load(descriptor.AssemblyName);
                    var resourceNames = assembly.GetManifestResourceNames();
                    var assemblyName = descriptor.AssemblyName.Name + ".";

                    foreach (var resourceName in resourceNames)
                    {
                        var builder =
                            new StringBuilder($"Areas/{descriptor.AreaName}/{resourceName.Replace(assemblyName, "")}");
                        builder.Replace(".", "/");
                        var extensionIndex = builder.ToString().LastIndexOf('/');
                        builder.Remove(extensionIndex, 1);
                        builder.Insert(extensionIndex, ".");
                        var path = builder.ToString();
                        var fileInfo = new EmbeddedResourceInfo(assembly, resourceName, Path.GetFileName(resourceName),
                            lastModified);
                        embeddedFileInfoCache.TryAdd(path, fileInfo);
                    }
                }
            }
        }

        public IDirectoryContents GetDirectoryContents(string subpath)
        {
            return physicalFileProvider.GetDirectoryContents(subpath);
        }

        public IChangeToken Watch(string filter)
        {
            return physicalFileProvider.Watch(filter);
        }
    }
}