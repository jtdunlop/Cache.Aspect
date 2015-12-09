// Heavily modified version of Postsharp.Cache https://www.nuget.org/packages/PostSharp.Cache

namespace DbSoft.Cache.Aspect
{
    using System.Collections.Generic;
    using Dbsoft.Cache.Aspect.Core;

    public static class CacheService
    {
		public static string SessionProperty { get; set; }
        public static ICacheProviderFactory CacheProviderFactory { get; set; }

        private static readonly List<ICache> Caches = new List<ICache>();

        public static ICache GetCache(string name)
        {
            var cache = Caches.Find(f => f.Name == name);
            if (cache != null) return cache;
            var provider = CacheProviderFactory.CreateCacheProvider(name);
            Caches.Add(provider);
            return provider;
        }

        public static void ClearAllCaches()
        {
            foreach ( var cache in Caches )
            {
                cache.DeleteAll();
            }
        }

        public static void ClearCache(string name)
        {
            var cache = Caches.Find(f => f.Name == name);
            cache?.DeleteAll();
        }
    }
}
