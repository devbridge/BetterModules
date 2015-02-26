using System;
using BetterModules.Core.DataContracts;
using NHibernate;

// ReSharper disable CheckNamespace
namespace BetterModules.Events
// ReSharper restore CheckNamespace
{   
    public class EntitySavingEventArgs : EventArgs
    {
        public IEntity Entity { get; set; }

        public ISession Session { get; set; }

        public EntitySavingEventArgs(IEntity entity, ISession session)
        {
            Entity = entity;
            Session = session;
        }
    }
}
