namespace BetterModules.Core.DataAccess.DataContext
{
    public interface IUnitOfWorkFactory
    {
        IUnitOfWork New();
    }
}
