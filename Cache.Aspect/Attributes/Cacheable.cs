// Heavily modified version of Postsharp.Cache https://www.nuget.org/packages/PostSharp.Cache

namespace DbSoft.Cache.Aspect.Attributes
{
    using System;
    using System.Diagnostics;
    using System.Reflection;
    using NLog;
    using PostSharp.Aspects;
    using Supporting;

    public static partial class Cache
    {
        [Serializable]
        public class Cacheable : OnMethodBoundaryAspect
        {
            private readonly int _ttl;
            private KeyBuilder _keyBuilder;
			private DateTime _start;

	        private KeyBuilder KeyBuilder
            {
                get { return _keyBuilder ?? (_keyBuilder = new KeyBuilder()); }
            }

            public Cacheable(int ttl = 0)
            {
                _ttl = ttl;
            }

            //Method executed at build time.
            public override void CompileTimeInitialize(MethodBase method, AspectInfo aspectInfo)
            {
                KeyBuilder.MethodParameters = method.GetParameters();
	            Debug.Assert(method.DeclaringType != null, "method.DeclaringType != null");
	            KeyBuilder.MethodName = string.Format("{0}.{1}", method.DeclaringType.FullName, method.Name);
            }

            // This method is executed before the execution of target methods of this aspect.
            public override void OnEntry(MethodExecutionArgs args)
            {
               // Compute the cache key.
				KeyBuilder.SessionProperty = CacheService.SessionProperty;
                var cacheKey = KeyBuilder.BuildCacheKey(args.Arguments);

                // Fetch the value from the cache.
                var cache = CacheService.Cache;
                var value = (DateWrapper<object>)(cache.Contains(cacheKey) ? cache[cacheKey] : null);

                if (value != null && !IsTooOld(value.Timestamp))
                {
                    // The value was found in cache. Don't execute the method. Return immediately.
                    args.ReturnValue = value.Object;
                    args.FlowBehavior = FlowBehavior.Return;
                }
                else
                {
                    if ( value != null )
                    {
                        cache.Delete(cacheKey);
                    }
					_start = DateTime.Now;
                    // The value was NOT found in cache. Continue with method execution, but store
                    // the cache key so that we don't have to compute it in OnSuccess.
                    args.MethodExecutionTag = cacheKey;
                }
            }

            // This method is executed upon successful completion of target methods of this aspect.
            public override void OnSuccess(MethodExecutionArgs args)
            {
                var cacheKey = (string)args.MethodExecutionTag;
                CacheService.Cache[cacheKey] = new DateWrapper<Object>
	               {
                    Object = args.ReturnValue,
                    Timestamp = DateTime.UtcNow
                };
				var elapsed = DateTime.Now - _start;
				if (elapsed.Seconds > .5)
				{
					LogManager.GetLogger("CacheAspect").Trace("{0}.{1} Execution Time: {2}", args.Method.DeclaringType, args.Method.Name, DateTime.Now - _start);
				}
            }

            private bool IsTooOld(DateTime time)
            {
                if ( _ttl == 0 )
                {
                    return false;
                }
                return DateTime.UtcNow - time > TimeSpan.FromSeconds(_ttl);                
            }

        }
    }

}

