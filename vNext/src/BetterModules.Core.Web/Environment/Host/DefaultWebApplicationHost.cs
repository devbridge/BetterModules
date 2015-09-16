using System;
using System.Web;
using BetterModules.Core.DataAccess.DataContext.Migrations;
using BetterModules.Core.Exceptions;
using BetterModules.Core.Web.Modules.Registration;

namespace BetterModules.Core.Web.Environment.Host
{
    /// <summary>
    /// Default web host implementation.
    /// </summary>
    [Obsolete("DefaultWebApplicationHost is deprecated. Consider utilizing DefaultWebApplicationAutoHost")]        
    public class DefaultWebApplicationHost : IWebApplicationHost
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultWebApplicationHost" /> class.
        /// </summary>
        /// <param name="modulesRegistration">The modules registration.</param>
        /// <param name="migrationRunner">The migration runner.</param>
        public DefaultWebApplicationHost(IWebModulesRegistration modulesRegistration, IMigrationRunner migrationRunner)
        {
        }

        /// <summary>
        /// Called when the host application starts.
        /// </summary>
        /// <param name="application">The host application.</param>
        /// <param name="validateViewEngines">if set to <c>true</c> valdiate view engines.</param>
        /// <exception cref="CoreException">ViewEngines.Engines collection doesn't contain any precompiled MVC view engines. Each module uses precompiled MVC engines for rendering views. Please check if Engines list is not cleared manualy in global.asax.cx</exception>
        public virtual void OnApplicationStart(HttpApplication application, bool validateViewEngines = true)
        {
        }

        /// <summary>
        /// Called when the host application stops.
        /// </summary>
        /// <param name="application">The host application.</param>
        public virtual void OnApplicationEnd(HttpApplication application)
        {
        }

        /// <summary>
        /// Called when the host application throws unhandled error.
        /// </summary>
        /// <param name="application">The host application.</param>
        public virtual void OnApplicationError(HttpApplication application)
        {
        }
        
        /// <summary>
        /// Called when the host application ends a web request.
        /// </summary>
        /// <param name="application">The host application.</param>
        public virtual void OnEndRequest(HttpApplication application)
        {
        }
        
        /// <summary>
        /// Called when the host application begins a web request.
        /// </summary>
        /// <param name="application">The host application.</param>
        public virtual void OnBeginRequest(HttpApplication application)
        {
        }

        /// <summary>
        /// Called when the host application authenticates a web request.
        /// </summary>
        /// <param name="application"></param>
        /// <exception cref="System.NotImplementedException"></exception>
        public virtual void OnAuthenticateRequest(HttpApplication application)
        {
        }

        /// <summary>
        /// Restarts and reloads application.
        /// </summary>
        /// <param name="application">The application.</param>
        public virtual void RestartAndReloadHost(HttpApplication application)
        {
        }

        /// <summary>
        /// Terminates current application. The application restarts on the next time a request is received for it.
        /// </summary>
        public virtual void RestartApplicationHost()
        {
        }
    }
}