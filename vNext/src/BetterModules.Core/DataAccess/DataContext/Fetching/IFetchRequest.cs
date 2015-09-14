using System.Linq;

namespace BetterModules.Core.DataAccess.DataContext.Fetching
{
    public interface IFetchRequest<TQueried, TFetch> : IOrderedQueryable<TQueried>
    {
    }
}
