using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NHibernate.Linq;

namespace BetterModules.Core.DataAccess.DataContext.Fetching
{
    public class FetchRequest<TQueried, TFetch> : IFetchRequest<TQueried, TFetch>
    {
        public IEnumerator<TQueried> GetEnumerator()
        {
            return NhFetchRequest.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return NhFetchRequest.GetEnumerator();
        }

        public Type ElementType => NhFetchRequest.ElementType;

        public Expression Expression => NhFetchRequest.Expression;

        public IQueryProvider Provider => NhFetchRequest.Provider;

        public FetchRequest(INhFetchRequest<TQueried, TFetch> nhFetchRequest)
        {
            NhFetchRequest = nhFetchRequest;
        }

        public INhFetchRequest<TQueried, TFetch> NhFetchRequest { get; }
    }
}
