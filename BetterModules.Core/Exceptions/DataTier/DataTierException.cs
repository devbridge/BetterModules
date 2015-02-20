using System;

namespace BetterModules.Core.Exceptions.DataTier
{
    [Serializable]
    public class DataTierException : CoreException
    {
        public DataTierException(string message)
            : base(message)
        {
        }

        public DataTierException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
