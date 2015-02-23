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
                RunPreStartMethods();

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
        private static void RunPreStartMethods()
        {
            var attribs = Assemblies.Concat(AppCodeAssemblies)
                                    .SelectMany(GetPreStartAttributes)
                                    .OrderBy(att => att.Order);

            foreach (var activationAttrib in attribs)
            {
                activationAttrib.InvokeMethod();
            }
        }

        private static IEnumerable<WebApplicationPreStartAttribute> GetPreStartAttributes(Assembly assembly)
        {
            try
            {
                return assembly.GetCustomAttributes(
                    typeof(WebApplicationPreStartAttribute),
                    false).OfType<WebApplicationPreStartAttribute>();
            }
            catch
            {
                // In some very odd (and not well understood) cases, GetCustomAttributes throws. Just ignore it.
                // See https://github.com/davidebbo/WebActivator/issues/12 for details
                return Enumerable.Empty<WebApplicationPreStartAttribute>();
            }
        }
    }
}
