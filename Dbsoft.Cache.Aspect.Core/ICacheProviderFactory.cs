namespace Dbsoft.Cache.Aspect.Core
{
    public interface ICacheProviderFactory
    {
        ICache CreateCacheProvider(string name);
    }
}