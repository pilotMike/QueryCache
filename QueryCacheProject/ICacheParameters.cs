namespace QueryCacheProject
{
    public interface ICacheParameters<T>
    {
        T UsingKeys(params object[] keys);
    }
}