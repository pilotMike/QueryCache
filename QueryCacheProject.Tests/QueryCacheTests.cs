using System;
using System.Linq;
using System.Linq.Expressions;
using Xunit;

namespace QueryCacheProject.Tests
{
    public class QueryCacheTests
    {
        private readonly QueryCache _cache;

        public QueryCacheTests()
        {
            _cache = new QueryCache();
        }
        [Fact]
        public void throws_exception_on_null()
        {
            Expression<Func<int>> f = null;
            Assert.Throws(typeof(ArgumentNullException), () => _cache.GetOrAdd(f));
        }

        [Fact]
        public void returns_data()
        {
            var result = _cache.GetOrAdd(() => 1);
            Assert.Equal(1, result);
        }

        [Fact]
        public void expression_returns_data_once()
        {
            const int iters = 100;
            var counter = new FunctionCallCounter();

            for(int i = 0; i < iters; i++)
            {
                _cache.GetOrAdd(() => counter.GetCallCount(0));
            }

            var result = counter.TotalCount;
            Assert.Equal(1, result);
        }

        [Fact]
        public void func_returns_data_once()
        {
            const int iters = 100;
            var counter = new FunctionCallCounter();

            for (int i = 0; i < iters; i++)
            {
                _cache.GetOrAddFunc(() => counter.GetCallCount(0)).UsingKeys(0);
            }

            var result = counter.TotalCount;
            Assert.Equal(1, result);
        }

        [Fact]
        public void returns_new_data_when_new_arguments_are_passed()
        {
            const int iters = 100;
            var counter = new FunctionCallCounter();

            for (int i = 0; i < iters; i++)
            {
                _cache.GetOrAdd(() => counter.GetCallCount(i));
            }

            var result = counter.TotalCount;
            Assert.Equal(iters, result);
        }

        [Fact]
        public void function_returns_new_data_when_new_arguments_are_passed()
        {
            const int iters = 100;
            var counter = new FunctionCallCounter();

            for (int i = 0; i < iters; i++)
            {
                _cache.GetOrAddFunc(() => counter.GetCallCount(i)).UsingKeys(i);
            }

            var result = counter.TotalCount;
            Assert.Equal(iters, result);
        }

        [Fact]
        public void caches_functions_with_custom_keys()
        {
            Func<int> f = () => 10;

            var cachedValue = _cache.GetOrAddFunc(f).UsingKeys(1);

            Assert.Equal(10, cachedValue);
        }

        [Fact]
        public void parallel_functions_with_same_key_dont_throw()
        {
            Func<int> f = () => 1;

            var results = ParallelEnumerable.Range(0, 100)
                .Select(i => _cache.GetOrAddFunc(f).UsingKeys(1))
                .AsEnumerable()
                .OrderBy(x => x)
                .ToList();

            var expected = Enumerable.Repeat(1, 100).ToArray();

            Assert.Equal(expected, results);

        }

        private class FunctionCallCounter
        {
            public int TotalCount => count;
            private int count = 0;
            public int GetCallCount(int x = 0)
            {
                count++;
                return count;
            }
        }
    }
}
