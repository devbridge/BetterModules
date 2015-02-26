using System;
using System.IO;
using System.Reflection;
using BetterModules.Core.Modules;

namespace BetterModules.NugetPackage
{
    /// <summary>
    /// Fake program to force solution to build NuGet package.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// Mains the specified args.
        /// </summary>
        /// <param name="args">The args.</param>
        public static void Main(string[] args)
        {
            var nugetTemplateFile = args[0];    // BetterModules.nuspec.template;
            var nugetFile = args[1];            // BetterModules.nuspec;

            var version = GetVersion();

            using (StreamReader sr = new StreamReader(nugetTemplateFile))
            {
                try
                {
                    var templateFile = sr.ReadToEnd();
                    templateFile = templateFile.Replace("@BetterModulesVersion@", version);

                    using (StreamWriter sw = new StreamWriter(nugetFile))
                    {
                        try
                        {
                            sw.Write(templateFile);
                        }
                        finally
                        {
                            sw.Close();
                        }
                    }
                }
                finally
                {
                    sr.Close();
                }
            }
        }

        private static string GetVersion()
        {
            var coreAssembly = typeof(ModuleDescriptor).Assembly;
            var assemblyInformationVersion = Attribute.GetCustomAttributes(coreAssembly, typeof(AssemblyInformationalVersionAttribute));

            if (assemblyInformationVersion.Length > 0)
            {
                var informationVersion = ((AssemblyInformationalVersionAttribute)assemblyInformationVersion[0]);

                return informationVersion.InformationalVersion;
            }

            return coreAssembly.GetName().Version.ToString(3);
        }
    }
}
