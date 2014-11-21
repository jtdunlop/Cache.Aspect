namespace DbSoft.Cache.Aspect.Supporting
{
    using System;

    [Serializable]
    public class DateWrapper<T>
    {
        public T Object
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
