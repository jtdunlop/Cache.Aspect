﻿namespace DbSoft.Cache.Aspect
{
    public interface ICache
    {
        object this[string key] { get; set; }

        bool Contains(string key);

        void Delete(string key);

		void DeleteAll(string group);
        void DeleteAll();
    }
}
