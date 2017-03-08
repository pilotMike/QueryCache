using System;
using System.Linq;

namespace QueryCacheProject
{
    internal class MemCacheKey
    {
        private readonly Type _returnType;
        private readonly object[] _keys;

        public MemCacheKey(Type returnType, object[] keys)
        {
            _returnType = returnType;
            _keys = keys;
        }

        /// <summary>
        /// Retuns a string representation of the return type and all of the arguments used 
        /// in the method call.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return _returnType.FullName + string.Join("", _keys.Select(key => key.ToString()));
        }
    }
}
