using System;

namespace BetterModules.Core.Exceptions
{
    /// <summary>
    /// Generic BetterModules Core exception.
    /// </summary>
    [Serializable]
    public class CoreException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CoreException" /> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public CoreException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CoreException" /> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public CoreException(string message, Exception innerException) : base(message, innerException)
        {
        }        
    }
}
