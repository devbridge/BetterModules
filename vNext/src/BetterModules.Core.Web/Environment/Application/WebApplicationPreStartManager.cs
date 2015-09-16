using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Web;
using System.Web.Compilation;
using System.Web.Hosting;
using BetterModules.Core.Web.Environment.Application;
using BetterModules.Core.Web.Extensions;

[assembly: PreApplicationStartMethod(typeof(WebApplicationPreStartManager), "PreApplicationStart")]

namespace BetterModules.Core.Web.Environment.Application
{
    /// <summary>
    /// Based on: https://github.com/davidebbo/WebActivator
    /// </summary>
    public class WebApplicationPreStartManager
    {
        private static bool _hasInited;
        private static List<Assembly> _assemblies;

        // For unit test purpose
        public static void Reset()
        {
            _hasInited = false;
            _assemblies = null;
        }

        public static void PreApplicationStart()
        {
            if (!_hasInited)
            {
                // Run pre-start methods
                RunPreStartMethods();

                // Register post-start methods to run after App_Start
                foreach (var type in GetWebApplicationHostTypes())
                {
                    Microsoft.Web.Infrastructure.DynamicModuleHelper.DynamicModuleUtility.RegisterModule(type);
                }

                _hasInited = true;
            }
        }

        private static IEnumerable<Assembly> Assemblies
        {
            get
            {
                if (_assemblies == null)
                {
                    // Cache the list of relevant assemblies, since we need it for both Pre and Post
                    _assemblies = new List<Assembly>();
                    foreach (var assemblyFile in GetAssemblyFiles())
                    {
                        try
                        {
                            // Ignore assemblies we can't load. They could be native, etc...
                            _assemblies.Add(Assembly.LoadFrom(assemblyFile));
                        }
                        catch (Win32Exception) { }
                        catch (ArgumentException) { }
                        catch (FileNotFoundException) { }
                        catch (PathTooLongException) { }
                        catch (BadImageFormatException) { }
                        catch (SecurityException) { }
                    }
                }

                return _assemblies;
            }
        }

        private static IEnumerable<string> GetAssemblyFiles()
        {
            // When running under ASP.NET, find assemblies in the bin folder.
            // Outside of ASP.NET, use whatever folder WebActivator itself is in
            string directory = HostingEnvironment.IsHosted
                ? HttpRuntime.BinDirectory
                : Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);
            return Directory.GetFiles(directory, "*.dll");
        }

        // Return all the App_Code assemblies
        private static IEnumerable<Assembly> AppCodeAssemblies
        {
            get
            {
                // Return an empty list if we;re not hosted or there aren't any
                if (!HostingEnvironment.IsHosted || !_hasInited || BuildManager.CodeAssemblies == null)
                {
                    return Enumerable.Empty<Assembly>();
                }

                return BuildManager.CodeAssemblies.OfType<Assembly>();
            }
        }

        // Call the relevant activation method from all assemblies
        private static void RunPreStartMethods(bool designerMode = false)
        {
            RunActivationMethods<WebApplicationPreStartAttribute>(designerMode);
        }

        private static void RunActivationMethods<T>(bool designerMode = false) where T : BaseActivationMethodAttribute
        {
            var attribs = Assemblies.Concat(AppCodeAssemblies)
                .SelectMany(assembly => assembly.GetAttributes<T>())
                .OrderBy(att => att.Order);

            foreach (var activationAttrib in attribs)
            {
                if (!designerMode || activationAttrib.ShouldRunInDesignerMode())
                {
                    activationAttrib.InvokeMethod();
                }
            }
        }

        static IEnumerable<Type> GetWebApplicationHostTypes()
        {
            return Assemblies.Concat(AppCodeAssemblies)
                .SelectMany(assembly => assembly.GetAttributes<WebApplicationHostAttribute>())
                .OrderBy(att => att.Order)
                .Select(att => att.Type);
        }
    }
}
