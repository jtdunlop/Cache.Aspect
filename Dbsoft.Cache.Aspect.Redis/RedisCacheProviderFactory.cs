namespace Dbsoft.Cache.Aspect.Redis
{
    using Core;

    public class RedisCacheProviderFactory : ICacheProviderFactory
    {
        private readonly string _endpoint;
        private readonly string _access;

        public RedisCacheProviderFactory(string endpoint, string access)
        {
            _endpoint = endpoint;
            _access = access;
        }

        public ICache CreateCacheProvider(string name)
        {
            return new RedisCacheProvider(_endpoint, _access, name);
        }
    }
}