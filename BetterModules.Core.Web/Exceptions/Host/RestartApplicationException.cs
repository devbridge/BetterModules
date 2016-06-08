using System;
using BetterModules.Core.Exceptions;

namespace BetterModules.Core.Web.Exceptions.Host
{
    [Serializable]
    public class RestartApplicationException : CoreException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RestartApplicationException" /> class.
        /// </summary>
        public RestartApplicationException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RestartApplicationException" /> class.
        /// </summary>
        /// <param name="message">The message</param>
        public RestartApplicationException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RestartApplicationException" /> class.
        /// </summary>
        /// <param name="message">The message</param>
        /// <param name="innerException">The inner exception</param>
        public RestartApplicationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
