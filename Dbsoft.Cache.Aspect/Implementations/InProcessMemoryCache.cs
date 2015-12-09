namespace CacheAspect.Implementations
{
	using System;
	using System.Collections.Generic;

	public class InProcessMemoryCache : ICache
    {
        private readonly Dictionary<string, object> cache = new Dictionary<string, object>();

        public object this[string key]
        {
            get
            { 
                return cache[key]; 
            }
            set
            {
                cache[key] = value;
            }
        }

        public bool Contains(string key)
        {
            return cache.ContainsKey(key);
        }

        public void Delete(string key)
        {
            cache.Remove(key);
        }

		public void DeleteAll(string @group)
		{
			throw new NotImplementedException();
		}

		public void DeleteAll()
	    {
		    throw new NotImplementedException();
	    }
    }
}
