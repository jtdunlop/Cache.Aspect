// Heavily modified version of Postsharp.Cache https://www.nuget.org/packages/PostSharp.Cache

namespace Dbsoft.Cache.Aspect.Core
{
    public interface ICache
    {
        string Name { get; }
        object this[string key] { set; }

        object Get(string key); 

        bool Contains(string key);

        void Delete(string key);

		void DeleteAll(string group);
        void DeleteAll();
    }
}
