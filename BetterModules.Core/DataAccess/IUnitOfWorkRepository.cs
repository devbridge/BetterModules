using BetterModules.Core.DataAccess.DataContext;

namespace BetterModules.Core.DataAccess
{ 
    public interface IUnitOfWorkRepository
    {        
        void Use(IUnitOfWork unitOfWork);
    }
}
