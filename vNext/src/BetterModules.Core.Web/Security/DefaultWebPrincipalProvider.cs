using System.Security.Principal;
using BetterModules.Core.Security;
using Microsoft.AspNet.Http;

namespace BetterModules.Core.Web.Security
{
    public class DefaultWebPrincipalProvider : DefaultPrincipalProvider
    {
        /// <summary>
        /// The HTTP context accessor
        /// </summary>
        private readonly IHttpContextAccessor httpContextAccessor;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultWebPrincipalProvider"/> class.
        /// </summary>
        /// <param name="httpContextAccessor">The HTTP context accessor.</param>
        public DefaultWebPrincipalProvider(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Gets the current principal.
        /// </summary>
        /// <returns>
        /// Current IPrincipal.
        /// </returns>
        public override IPrincipal GetCurrentPrincipal()
        {
            var currentHttpContext = httpContextAccessor.HttpContext;

            if (currentHttpContext == null)
            {
                return base.GetCurrentPrincipal();
            }

            return currentHttpContext.User;
        }
    }
}
