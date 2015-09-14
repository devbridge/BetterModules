using System;

namespace BetterModules.Core.Exceptions.DataTier
{
    [Serializable]
    public class EntityNotFoundException : DataTierException
    {
        public EntityNotFoundException(string message)
            : base(message)
        {
        }

        public EntityNotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public EntityNotFoundException(Type entityType, Guid id)
            : base($"{entityType.Name} entity not found by id={id}.")
        {
        }

        public EntityNotFoundException(Type entityType, string filter)
            : base($"{entityType.Name} entity not found by filter {filter}.")
        {
        }
    }
}
