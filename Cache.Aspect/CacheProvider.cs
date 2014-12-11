// Heavily modified version of Postsharp.Cache https://www.nuget.org/packages/PostSharp.Cache

namespace DbSoft.Cache.Aspect
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Caching;

    // Key structure:
    // guid(session).keyname.serialization
    // if no session and no keyname, ..serialization
    // Delete by session: starts with session + '.';
    // Delete by key: contains '.' + keyname + '.';

    public class CacheProvider : ICache
    {
        public static CacheProvider Cache;

        public CacheProvider()
        {
            _cache = MemoryCache.Default;
            _keys = new HashSet<string>();
        }

        public bool Contains(string key)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // return false;
            }
            return _cache[key] != null;
        }

        public void Delete(string key)
        {
            _cache.Remove(key);
            if (_keys.Contains(key))
            {
                _keys.Remove(key);
            }
        }

        public void DeleteAll(string @session)
        {
            ClearAll(session);
        }

        public void DeleteAll()
        {
            ClearAll(null);
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
                if (!_keys.Contains(key))
                {
                    _keys.Add(key);
                }
            }
        }

        private void ClearAll(string searchKey)
        {
            if (string.IsNullOrEmpty(searchKey))
            {
                foreach (var key in _keys)
                {
                    _cache.Remove(key);
                }
                _keys.Clear();
            }
            else
            {
                var keys = _keys.Where(k => k.IndexOf(searchKey, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
                foreach (var key in keys)
                {
                    _keys.Remove(key);
                    _cache.Remove(key);
                }
            }
        }
        private readonly ObjectCache _cache;
        private readonly HashSet<string> _keys;
    }
}
