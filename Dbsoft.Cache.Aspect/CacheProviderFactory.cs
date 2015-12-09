namespace DbSoft.Cache.Aspect
{
    using Dbsoft.Cache.Aspect.Core;

    public class CacheProviderFactory : ICacheProviderFactory
    {
        public ICache CreateCacheProvider(string name)
        {
            return new CacheProvider(name);
        }
    }
}