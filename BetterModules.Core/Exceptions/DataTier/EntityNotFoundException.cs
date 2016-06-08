using System;

namespace BetterModules.Core.Exceptions.DataTier
{
    [Serializable]
    public class EntityNotFoundException : DataTierException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityNotFoundException" /> class.
        /// </summary>
        public EntityNotFoundException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityNotFoundException" /> class.
        /// </summary>
        /// <param name="message">The message</param>
        public EntityNotFoundException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityNotFoundException" /> class.
        /// </summary>
        /// <param name="message">The message</param>
        /// <param name="innerException">The inner exception</param>
        public EntityNotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityNotFoundException" /> class.
        /// </summary>
        /// <param name="entityType">The entity type</param>
        /// <param name="id">The id of the entity</param>
        public EntityNotFoundException(Type entityType, Guid id)
            : base(string.Format("{0} entity not found by id={1}.", entityType.Name, id))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityNotFoundException" /> class.
        /// </summary>
        /// <param name="entityType">The entity type</param>
        /// <param name="filter">The filter</param>
        public EntityNotFoundException(Type entityType, string filter)
            : base(string.Format("{0} entity not found by filter {1}.", entityType.Name, filter))
        {
        }
    }
}
