using System;

namespace QueryCacheProject
{
    internal class CacheParameters<T> : ICacheParameters<T>
    {
        private readonly QueryCache _queryCache;
        private readonly Func<T> _func;

        public CacheParameters(QueryCache queryCache, Func<T> func)
        {
            _queryCache = queryCache;
            _func = func;
        }

        public T UsingKeys(params object[] keys)
        {
            var key = new MemCacheKey(typeof(T), keys);
            var lazyFunc = new Lazy<T>(_func);
            return _queryCache.GetValue(key, lazyFunc);
        }
    }
}