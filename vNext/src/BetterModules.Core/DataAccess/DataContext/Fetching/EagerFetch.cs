using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NHibernate.Linq;

namespace BetterModules.Core.DataAccess.DataContext.Fetching
{
    public static class EagerFetch
    {
        public static IFetchRequest<TOriginating, TRelated> Fetch<TOriginating, TRelated>(
            this IQueryable<TOriginating> query, Expression<Func<TOriginating, TRelated>> relatedObjectSelector)
        {
            var fetch = EagerFetchingExtensionMethods.Fetch(query, relatedObjectSelector);
            return new FetchRequest<TOriginating, TRelated>(fetch);
        }

        public static IFetchRequest<TOriginating, TRelated> FetchMany<TOriginating, TRelated>(
            this IQueryable<TOriginating> query,
            Expression<Func<TOriginating, IEnumerable<TRelated>>> relatedObjectSelector)
        {
            var fetch = EagerFetchingExtensionMethods.FetchMany(query, relatedObjectSelector);
            return new FetchRequest<TOriginating, TRelated>(fetch);
        }

        public static IFetchRequest<TQueried, TRelated> ThenFetch<TQueried, TFetch, TRelated>(
            this IFetchRequest<TQueried, TFetch> query, Expression<Func<TFetch, TRelated>> relatedObjectSelector)
        {
            var impl = query as FetchRequest<TQueried, TFetch>;
            var fetch = impl.NhFetchRequest.ThenFetch(relatedObjectSelector);
            return new FetchRequest<TQueried, TRelated>(fetch);
        }

        public static IFetchRequest<TQueried, TRelated> ThenFetchMany<TQueried, TFetch, TRelated>(
            this IFetchRequest<TQueried, TFetch> query,
            Expression<Func<TFetch, IEnumerable<TRelated>>> relatedObjectSelector)
        {
            var impl = query as FetchRequest<TQueried, TFetch>;
            var fetch = impl.NhFetchRequest.ThenFetchMany(relatedObjectSelector);
            return new FetchRequest<TQueried, TRelated>(fetch);
        }
    }
}