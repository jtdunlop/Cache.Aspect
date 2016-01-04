// Heavily modified version of Postsharp.Cache https://www.nuget.org/packages/PostSharp.Cache

namespace DbSoft.Cache.Aspect.Attributes
{
    using System;
    using System.Diagnostics;
    using System.Reflection;
    using Newtonsoft.Json;
    using PostSharp.Aspects;
    using Supporting;

    public static partial class Cache
    {
        [Serializable]
        public class Cacheable : OnMethodBoundaryAspect
        {
            private readonly int _ttl;
            private readonly string _name;
            private KeyBuilder _keyBuilder;

	        private KeyBuilder KeyBuilder => _keyBuilder ?? (_keyBuilder = new KeyBuilder());

            public Cacheable(int ttl = 0, string name = "")
            {
                _ttl = ttl;
                _name = name;
            }

            //Method executed at build time.
            public override void CompileTimeInitialize(MethodBase method, AspectInfo aspectInfo)
            {
                KeyBuilder.MethodParameters = method.GetParameters();
	            Debug.Assert(method.DeclaringType != null, "method.DeclaringType != null");
	            KeyBuilder.MethodName = $"{method.DeclaringType.FullName}.{method.Name}";
            }

            // This method is executed before the execution of target methods of this aspect.
            public override void OnEntry(MethodExecutionArgs args)
            {
                var info = (MethodInfo)args.Method;
                
                KeyBuilder.SessionProperty = CacheService.SessionProperty;
                var cacheKey = KeyBuilder.BuildCacheKey(args.Arguments);
                try
                {
                    var cache = CacheService.GetCache(CacheName(cacheKey.Token));
                    var entry = cache.Contains(cacheKey.Key) ? cache.Get(cacheKey.Key) : null;

                    if (entry == null)
                    {
                        args.MethodExecutionTag = cacheKey;
                        return;
                    }

                    var value = (DateWrapper)entry;

                    if (!IsTooOld(value.Timestamp))
                    {
                        // The value was found in cache. Don't execute the method. Return immediately.
                        args.ReturnValue = value.Object;
                        args.FlowBehavior = FlowBehavior.Return;
                    }
                    else
                    {
                        if (value != null)
                        {
                            cache.Delete(cacheKey.Key);
                        }
                        // The value was NOT found in cache. Continue with method execution, but store
                        // the cache key so that we don't have to compute it in OnSuccess.
                        args.MethodExecutionTag = cacheKey;
                    }
                }
                catch (Exception)
                {
                    args.MethodExecutionTag = cacheKey;
                }
            }

            static bool IsIEnumerable(Type type)
            {
                return type.Name.Contains("IEnumerable");
            }

            private string CacheName(string token)
            {
                return string.IsNullOrWhiteSpace(_name) ? token : token + "." + _name;
            }

            // This method is executed upon successful completion of target methods of this aspect.
            public override void OnSuccess(MethodExecutionArgs args)
            {
                var cacheKey = (CacheKeyResult)args.MethodExecutionTag;
                var entry = new DateWrapper
                {
                    Object = args.ReturnValue,
                    Timestamp = DateTime.UtcNow
                };
                CacheService.GetCache(CacheName(cacheKey.Token))[cacheKey.Key] = entry;
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

