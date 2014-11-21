namespace CacheAspect.Implementations
{
	using System;
	using Microsoft.ApplicationServer.Caching;

	public class OutOfProcessMemoryCache : ICache
    {
        public OutOfProcessMemoryCache()
        {
            var configuration = new DataCacheFactoryConfiguration();
            var factory = new DataCacheFactory(configuration);
            _cache = factory.GetCache(CacheName);
        }
        
        public object this[string key]
        {
            get
            {
                return _cache[key]; 
            }
            set
            {
                _cache[key] = value;
            }
        }


        public bool Contains(string key)
        {
            //App Fabric Cache has no Contains method
            return _cache[key] != null;
        }


        public void Delete(string key)
        {
            _cache.Remove(key);
        }

		public void DeleteAll(string @group)
		{
			throw new NotImplementedException();
		}

		public void DeleteAll()
	    {
		    throw new NotImplementedException();
	    }

	    public static String CacheName = "CacheAttribute";
        private static DataCache _cache;
    }
}
