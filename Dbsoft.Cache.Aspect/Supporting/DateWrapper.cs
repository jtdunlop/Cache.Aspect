// Heavily modified version of Postsharp.Cache https://www.nuget.org/packages/PostSharp.Cache

namespace DbSoft.Cache.Aspect.Supporting
{
    using System;

    [Serializable]
    public class DateWrapper
    {
        public object Object
        {
            get;
            set;
        }
        public DateTime Timestamp
        {
            get;
            set;
        }
    }
}
