using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using BetterModules.Core.Exceptions;
using BetterModules.Core.Web.Exceptions.Host;
using Common.Logging;
using RazorGenerator.Mvc;

namespace BetterModules.Core.Web.Environment.Host
{
    public abstract class DefaultWebApplicationAutoHost : IWebApplicationAutoHost
    {
        private static readonly ILog _logger = LogManager.GetCurrentClassLogger();

        private static object _lock = new object();

        protected ILog Logger { get { return _logger; } }

        private string _hostName = "Web application host";

        public string HostName
        {
            get { return _hostName; }
            set { _hostName = value; }
        }

        protected int InitializedCount
        {
            get
            {
                var type = GetType();
                if (InitializedTypes.ContainsKey(type))
                {
                    return InitializedTypes[type];
                }
                InitializedTypes.Add(type, 0);
                return 0;
            }
            set
            {
                var type = GetType();
                if (!InitializedTypes.ContainsKey(type))
                {
                    InitializedTypes.Add(type, value);
                }
                else
                {
                    InitializedTypes[GetType()] = value;
                }
            }
        }

        private static readonly Dictionary<Type, int> InitializedTypes = new Dictionary<Type, int>();

        private HttpApplication _application;

        protected HttpApplication Application { get { return _application; } }

        public object Lock
        {
            get { return _lock; }
        }

        public virtual void Init(HttpApplication context)
        {
            _application = context;

            lock (Lock)
            {
                AttachApplicationEvents(_application);

                if(InitializedCount++ == 0)
                {
                    OnApplicationStart(_application);
                }
            }
        }

        public virtual void Dispose()
        {
            lock (_lock)
            {
                if (--InitializedCount == 0)
                {
                    OnApplicationEnd(_application);
                }
            }
        }

        private void AttachApplicationEvents(HttpApplication application)
        {
            application.AuthenticateRequest += Application_AuthenticateRequest;
            application.BeginRequest        += Application_BeginRequest;
            application.EndRequest          += Application_EndRequest;
            application.Error               += Application_Error;
        }

        public virtual void OnEndRequest(HttpApplication application)
        {
        }

        public virtual void OnAuthenticateRequest(HttpApplication application)
        {
        }

        public virtual void OnApplicationStart(HttpApplication application, bool validateViewEngines = true)
        {
            try
            {
                Logger.InfoFormat("{0} starting...", HostName);
                if (validateViewEngines && !ViewEngines.Engines.Any(engine => engine is CompositePrecompiledMvcEngine))
                {
                    throw new CoreException("ViewEngines.Engines collection doesn't contain precompiled composite MVC view engine. Application modules use precompiled MVC views for rendering. Please check if Engines list is not cleared manualy in global.asax.cx");
                }

                Logger.InfoFormat("{0} started.", HostName);
            }
            catch (Exception ex)
            {
                Logger.Fatal("Failed to start host application.", ex);
            }
        }

        public virtual void OnApplicationEnd(HttpApplication application)
        {
            Logger.InfoFormat("{0} stopped.", HostName);
        }

        public virtual void OnApplicationError(HttpApplication application)
        {
        }

        public virtual void OnBeginRequest(HttpApplication application)
        {
#if DEBUG
            // A quick way to restart an application host.
            // This is not going to affect production as it is compiled only in the debug mode.
            if (application.Request["restart"] == "1")
            {
                RestartAndReloadHost(application);
            }
#endif
        }

        public virtual void RestartApplicationHost()
        {
            try
            {
                HttpRuntime.UnloadAppDomain();
            }
            catch
            {
                try
                {
                    bool success = TryTouchBinRestartMarker() || TryTouchWebConfig();

                    if (!success)
                    {
                        throw new RestartApplicationException("Failed to terminate host application.");
                    }
                }
                catch (Exception ex)
                {
                    throw new RestartApplicationException("Failed to terminate host application.", ex);
                }
            }
        }

        private void Application_AuthenticateRequest(object sender, EventArgs e)
        {
            OnAuthenticateRequest((HttpApplication)sender);
        }

        private void Application_BeginRequest(object sender, EventArgs e)
        {
            OnBeginRequest((HttpApplication)sender);
        }

        private void Application_EndRequest(object sender, EventArgs e)
        {
            OnEndRequest((HttpApplication)sender);
        }

        private void Application_Error(object sender, EventArgs e)
        {
            OnApplicationError((HttpApplication)sender);
        }

        private bool TryTouchBinRestartMarker()
        {
            try
            {
                var binMarker = HostingEnvironment.MapPath("~/bin/restart");
                Directory.CreateDirectory(binMarker);

                using (var stream = File.CreateText(Path.Combine(binMarker, "marker.txt")))
                {
                    stream.WriteLine("Restarted on '{0}'", DateTime.UtcNow);
                    stream.Flush();
                }

                return true;
            }
            catch (Exception ex)
            {
                Logger.Warn("Failed to touch web host application \bin folder.", ex);
                return false;
            }
        }

        private void RestartAndReloadHost(HttpApplication application)
        {
            RestartApplicationHost();

            Thread.Sleep(500);

            UriBuilder uri = new UriBuilder(application.Request.Url);
            uri.Query = string.Empty;

            application.Response.ClearContent();
            application.Response.Write(string.Format("<script type=\"text/javascript\">window.location = '{0}';</script>", uri));
            application.Response.End();
        }

        private bool TryTouchWebConfig()
        {
            try
            {
                File.SetLastWriteTimeUtc(HostingEnvironment.MapPath("~/web.config"), DateTime.UtcNow);
                return true;
            }
            catch (Exception ex)
            {
                Logger.Warn("Failed to touch web host application web.config file.", ex);
                return false;
            }
        }
    }
}
