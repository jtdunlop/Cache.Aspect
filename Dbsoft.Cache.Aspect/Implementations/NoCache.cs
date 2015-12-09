// Heavily modified version of Postsharp.Cache https://www.nuget.org/packages/PostSharp.Cache

namespace DbSoft.Cache.Aspect.Implementations
{
    using System;

    //primarly used for unit tests
    public class NoCache : ICache
    {
        public object this[string key]
        {
            get
            {
                return null; 
            }
            set
            {
	            if (value == null) throw new ArgumentNullException("value");
            }
        }

        public bool Contains(string key)
        {
            return false;
        }

        public void Delete(string key)
        {
        }

	    public void DeleteAll(string @group)
	    {
	    }

        public void DeleteAll()
        {
        }
    }
}
