using System;

namespace BetterModules.Core.Exceptions.DataTier
{
    [Serializable]
    public class DataTierException : CoreException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataTierException" /> class.
        /// </summary>
        public DataTierException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataTierException" /> class.
        /// </summary>
        /// <param name="message">The message</param>
        public DataTierException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataTierException" /> class.
        /// </summary>
        /// <param name="message">The message</param>
        /// <param name="innerException">The inner exception</param>
        public DataTierException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
