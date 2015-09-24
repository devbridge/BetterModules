using System;
using System.Linq.Expressions;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;

namespace BetterModules.Core.Web.Web
{
    /// <summary>
    /// Defines the contract to access current http context.
    /// </summary>
    public interface IHttpContextAccessor: Microsoft.AspNet.Http.IHttpContextAccessor
    {
        /// <summary>
        /// Returns the physical file path that corresponds to the specified virtual path on the Web server.
        /// </summary>
        /// <param name="path">The virtual path of the Web server.</param>
        /// <returns>The physical file path that corresponds to path.</returns>
        string MapPath(string path);

        /// <summary>
        /// Returns the absolute path that corresponds to the virtual path on the Web server.
        /// </summary>
        /// <param name="path">The virtual path of the Web server.</param>
        /// <returns>The absolute path that corresponds to path.</returns>
        string MapPublicPath(string path);

        /// <summary>
        /// Resolves the action URL.
        /// </summary>
        /// <typeparam name="TController">The type of the controller.</typeparam>
        /// <param name="expression">The expression.</param>
        /// <param name="fullUrl">if set to <c>true</c> retrieve full URL.</param>
        /// <returns></returns>
        string ResolveActionUrl<TController>(Expression<Action<TController>> expression, bool fullUrl = false)
            where TController : Controller;
    }
}
