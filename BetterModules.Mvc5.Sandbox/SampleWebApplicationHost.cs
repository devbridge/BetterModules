using System;
using System.Web;

using BetterModules.Core.DataAccess.DataContext.Migrations;
using BetterModules.Core.Exceptions;
using BetterModules.Core.Web.Environment.Application;
using BetterModules.Core.Web.Environment.Host;
using BetterModules.Core.Web.Modules.Registration;
using BetterModules.Mvc5.Sandbox;

[assembly : WebApplicationHost(typeof(SampleWebApplicationHost), Order = 100)]
namespace BetterModules.Mvc5.Sandbox
{
    public class SampleWebApplicationHost : DefaultWebApplicationHost
    {
        public SampleWebApplicationHost(IWebModulesRegistration modulesRegistration, IMigrationRunner migrationRunner)
            : base(modulesRegistration, migrationRunner)
        {
        }

        HttpApplication SenderAsHttpApplication(object sender)
        {
            var application = sender as HttpApplication;
            if (sender == null)
            {
                throw new CoreException("Expected sender to be HttpApplication");
            }
            return application;
        }
    }
}