using System;
using BetterModules.Core.Exceptions;

namespace BetterModules.Core.Web.Exceptions.Host
{
    [Serializable]
    public class RestartApplicationException : CoreException
    {
        public RestartApplicationException(string message)
            : base(message)
        {
        }

        public RestartApplicationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
