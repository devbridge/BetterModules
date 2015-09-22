namespace BetterModules.Core.Infrastructure.Commands
{
    public interface ICommandResolver
    {
        TCommand ResolveCommand<TCommand>(ICommandContext context) where TCommand : ICommandBase;
    }
}
