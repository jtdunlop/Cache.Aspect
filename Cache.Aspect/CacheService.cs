namespace DbSoft.Cache.Aspect
{
    using System;
    using System.Configuration;
    using Implementations;

    public static class CacheService
    {
		public static string SessionProperty { get; set; }
        static CacheService()
        {
            InitDiskPath();
            InitCache();
        }

        private static void InitDiskPath()
        {
        }

		private static void InitCache()
        {
	        if (Cache != null) return;
			var setting = ConfigurationManager.AppSettings["CacheAspect.CacheType"];
			if (setting != null)
			{
				var type = Type.GetType(setting);
				if (type != null)
				{
					Cache = (ICache)Activator.CreateInstance(type);
					return;
				}
			}
			//if a cache is not configured, fall back on NoCache 
			//this happens is useful for test cases
			Cache = new NoCache();
        }

		public static ICache Cache;
    }
}
