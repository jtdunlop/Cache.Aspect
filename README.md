This is a heavily modified version of Postsharp.Cache, available on nuget at https://www.nuget.org/packages/PostSharp.Cache
and on github at https://github.com/agbell/attribute-based-caching. After using Postsharp.Cache in two production environments, 
I felt compelled to adapt it to meet the following objectives:

- Able to cache and flush a user session independent of other sessions.
- Support for ttl on a per-method basis
- Able to detect the difference between two complex request objects

Cache matching is based on comparing the deserialization of all parameters passed to a method to the deserialization of previous calls.
If a complex request passed and any property anywhere is different, it will be independently cached. This is obviously potentially
memory intensive, but performance is always a tradeoff against space. In practice, I have found that allocating more memory to 
allow repeated long running tasks to be preferable to saturating the data store.

At application startup:

CacheProvider.Cache = new CacheProvider();<br/>
CacheService.Cache = CacheProvider.Cache;<br/>
CacheService.SessionProperty = "Token";

To cache a method, simply decorate it with [Cache.Cacheable]. If the method has a parameter or object property matching 
CacheService.SessionProperty, it will use per-session matching; otherwise it will use global matching.

To set an expiry on a method, decorate it with [Cache.Cacheable(ttl)] where ttl is an integer of seconds.

CacheService.SessionProperty sets the name of the parameter or parameter property that identifies the session making a request.
Given the above example:

var request = new Request { Id = 1, Token = "x" };<br />
var response = service.Call(request);

This code will return a cached result identified by session token "x".

To flush a session cache, call a method decorated with [Cache.TriggerInvalidation(DeleteSettings.Token)] with the relevant session value
passed as a parameter or object property. To flush the system cache, call a method decorated with [Cache.TriggerInvalidation(DeleteSettings.All)]


