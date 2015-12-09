This is a heavily modified version of Postsharp.Cache, available on nuget at https://www.nuget.org/packages/PostSharp.Cache
and on github at https://github.com/agbell/attribute-based-caching. After using Postsharp.Cache in two production environments, 
I felt compelled to adapt it to meet the following objectives:

- Able to cache and flush a user session independent of other sessions.
- Support for ttl on a per-method basis
- Able to detect the difference between two complex request objects

Cache matching is based on comparing a hash of the method name in conjunction with the deserialization of all parameters passed to a method to 
the deserialization of previous calls. Additionally, cache segregation is supported via naming a parameter/property on which 
the cache should be segregated. The primary benefit of segregating data is to allow a portion of the cache to be flushed independent of others. 
Even if segregation isn't used, including a tenantid/userid in method calls will return distinct result sets
as the hash won't match. For the remainder of this document, tenantId will be assumed to be the named parameter/property

At application startup:

CacheService.SessionProperty = "tenantid"; // Case insensitive
CacheService.CacheProviderFactory = new CacheProviderFactory(); // RedisCacheProviderFactory is also included.

To cache a method, simply decorate it with [Cache.Cacheable]. If the method has a parameter or object property matching 
CacheService.SessionProperty, it will use per-session matching; otherwise it will use global matching.

To set an expiry on a method, decorate it with [Cache.Cacheable(ttl)] where ttl is an integer of seconds.

To flush a named cache, call CacheService.ClearCache(name). 
To flush the global cache, call CacheService.ClearCache("default"). 
To flush all caches, call CacheService.ClearAllCaches().


