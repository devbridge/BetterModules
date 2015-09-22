
using System;
using Microsoft.Framework.DependencyInjection;

namespace BetterModules.Core.Infrastructure.Commands
{
    public class DefaultCommandResolver : ICommandResolver
    {
        private readonly IServiceProvider serviceProvider;

        public DefaultCommandResolver(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public TCommand ResolveCommand<TCommand>(ICommandContext context) where TCommand : ICommandBase
        {
            var command = serviceProvider.GetService<TCommand>();
            if (command != null)
            {
                command.Context = context;
                return command;
            }

            return default(TCommand);
        }
    }
}
