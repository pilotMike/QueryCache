using System;
using System.Linq.Expressions;
using System.Runtime.Caching;

namespace QueryCacheProject
{
    public class QueryCache
    {
        private readonly ExpressionValueExtractor _expressionValueExtractor;
        private readonly MemoryCache _cache;

        public QueryCache()
        {
            _expressionValueExtractor = new ExpressionValueExtractor();
            _cache = new MemoryCache("emptyConfigName");
        }

        public T GetOrAdd<T>(Expression<Func<T>> expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            object[] parameters = _expressionValueExtractor.GetArgumentValues(expression);
            var key = new MemCacheKey(typeof(T), parameters);

            Func<T> funcToExecute = expression.Compile();
            var newLazyCacheItem = new Lazy<T>(funcToExecute);

            return GetValue(key, newLazyCacheItem);
        }

        public ICacheParameters<T> GetOrAddFunc<T>(Func<T> func)
        {
            if (func == null)
            {
                throw new ArgumentNullException(nameof(func));
            }
            return new CacheParameters<T>(this, func);
        }

        internal T GetValue<T>(MemCacheKey key, Lazy<T> newLazyCacheItem)
        {
            var existingCacheItem = (Lazy<T>)_cache.AddOrGetExisting(key.ToString(), newLazyCacheItem, new CacheItemPolicy());

            if (existingCacheItem != null)
            {
                return existingCacheItem.Value;
            }

            try
            {
                return newLazyCacheItem.Value;
            }
            catch // the provided function failed, so remove it from cache
            {
                _cache.Remove(key.ToString());
                throw;
            }
        }
    }
}
