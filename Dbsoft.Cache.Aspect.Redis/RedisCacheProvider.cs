namespace Dbsoft.Cache.Aspect.Redis
{
    using System.Linq;
    using Core;
    using ServiceStack.Redis;

    class RedisCacheProvider : ICache
    {
        public string Name { get; }

        readonly RedisClient _client;
        readonly RedisClient _native;

        public RedisCacheProvider(string endpoint, string access, string name)
        {
            _client = new RedisClient(endpoint, 6379, access);
            _native = new RedisClient(endpoint, 6379, access);
            Name = name;
        }

        public object this[string key]
        {
            set
            {
                var add = value;
                _client.Add(key, add);
                _native.SAdd(Name, System.Text.Encoding.Default.GetBytes(key));
            }
        }

        public object Get(string key)
        {
            var entry = _native.SMembers(Name).FirstOrDefault(f => System.Text.Encoding.Default.GetString(f) == key);
            var get = _client.Get<string>(key);
            return entry == null ? null : get;
        } 

        public bool Contains(string key)
        {
            return _native.SMembers(Name).FirstOrDefault(f => System.Text.Encoding.Default.GetString(f) == key) != null;
        }

        public void Delete(string key)
        {
            var entry = _native.SMembers(Name).FirstOrDefault(f => System.Text.Encoding.Default.GetString(f) == key);
            if (entry == null) return;
            _client.Remove(key);
            _native.SRem(Name, System.Text.Encoding.Default.GetBytes(key));

        }


        public void DeleteAll(string name)
        {
            foreach (var key in _native.SMembers(name).Select(member => System.Text.Encoding.Default.GetString(member)))
            {
                _client.Remove(key);
            }
        }

        public void DeleteAll()
        {
            DeleteAll(Name);
        }
    }
}
