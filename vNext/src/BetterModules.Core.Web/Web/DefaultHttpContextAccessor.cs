﻿using System;
using System.IO;
using System.Reflection;
using BetterModules.Core.Web.Configuration;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Http.Extensions;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Rendering.Expressions;

namespace BetterModules.Core.Web.Web
{
    /// <summary>
    /// Default implementation of http context accessor. Provides HttpContext.Current context wrapper.
    /// </summary>
    public class DefaultHttpContextAccessor : IHttpContextAccessor
    {
        /// <summary>
        /// The web configuration
        /// </summary>
        private readonly IWebConfiguration configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultHttpContextAccessor" /> class.
        /// </summary>
        /// <param name="configuration">The application configuration.</param>
        public DefaultHttpContextAccessor(IWebConfiguration configuration)
        {
            this.configuration = configuration;
        }

        // TBD: create a DefaultControllerContextAccessor service to get current controller views and etc. 

        public HttpContext GetCurrent()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the physical file path that corresponds to the specified virtual path on the Web server.
        /// </summary>
        /// <param name="path">The virtual path of the Web server.</param>
        /// <returns>
        /// The physical file path that corresponds to path.
        /// </returns>
        public string MapPath(string path)
        {
            var current = GetCurrent();
            if (current != null)
            {
                return current.Server.MapPath(path);
            }
            
            return Path.Combine(GetExecutingAssemblyPath(), path);
        }

        /// <summary>
        /// Returns the absolute path that corresponds to the virtual path on the Web server.
        /// </summary>
        /// <param name="path">The virtual path of the Web server.</param>
        /// <returns>The absolute path that corresponds to path.</returns>
        public string MapPublicPath(string path)
        {
            return string.Concat(GetServerUrl(GetCurrent().Request).TrimEnd('/'), VirtualPathUtility.ToAbsolute(path));
        }

        /// <summary>
        /// Resolves the action URL.
        /// </summary>
        /// <typeparam name="TController">The type of the controller.</typeparam>
        /// <param name="expression">The expression.</param>
        /// <param name="fullUrl">if set to <c>true</c> retrieve full URL.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public string ResolveActionUrl<TController>(System.Linq.Expressions.Expression<Action<TController>> expression, bool fullUrl = false) 
            where TController : Controller
        {
            var routeValuesFromExpression = ExpressionHelper.GetRouteValuesFromExpression(expression);
            var action = routeValuesFromExpression["Action"].ToString();
            var controller = routeValuesFromExpression["Controller"].ToString();
            var current = GetCurrent();
            if (current != null)
            {
                string url = new UrlHelper(null, null).Action(action, controller, routeValuesFromExpression);
                if (fullUrl)
                {
                    url = string.Concat(GetServerUrl(current.Request).TrimEnd('/'), url);
                }
                
                url = HttpUtility.UrlDecode(url);

                return url;
            }

            return null;
        }

        /// <summary>
        /// Gets the server URL.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        private string GetServerUrl(HttpRequest request)
        {
            // TODO: Check if this is the right way to get server Url
            if (request != null
                && string.IsNullOrWhiteSpace(configuration.WebSiteUrl) || configuration.WebSiteUrl.Equals("auto", StringComparison.InvariantCultureIgnoreCase))
            {
                var url = request?.Host.Value;
                //var query = HttpContext.Current.Request.Url.PathAndQuery;
                //if (!string.IsNullOrEmpty(query) && query != "/")
                //{
                //    url = url.Replace(query, null);
                //}

                return url;
            }

            return configuration.WebSiteUrl;
        }

        private string GetExecutingAssemblyPath()
        {
            string codeBase = Assembly.GetExecutingAssembly().CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);

            return Path.GetDirectoryName(uri.Path);
        }
    }
}
