using Autofac;
using BetterModules.Core.Web.Dependencies;

namespace BetterModules.Core.Web.Mvc.Commands
{
    public class DefaultCommandResolver : ICommandResolver
    {
        private readonly PerWebRequestContainerProvider containerProvider;

        public DefaultCommandResolver(PerWebRequestContainerProvider containerProvider)
        {
            this.containerProvider = containerProvider;
        }

        public TCommand ResolveCommand<TCommand>(ICommandContext context) where TCommand : ICommandBase
        {
            if (containerProvider.CurrentScope.IsRegistered<TCommand>())
            {
                var command = containerProvider.CurrentScope.Resolve<TCommand>();
                command.Context = context;                

                return command;
            }

            return default(TCommand);
        }
    }
}
