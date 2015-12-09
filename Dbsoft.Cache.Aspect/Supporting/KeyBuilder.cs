// Heavily modified version of Postsharp.Cache https://www.nuget.org/packages/PostSharp.Cache

namespace DbSoft.Cache.Aspect.Supporting
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using PostSharp.Aspects;

    [Serializable]
    public class KeyBuilder
    {
		public string SessionProperty { get; set; }
        public string MethodName { private get; set; }
        private Dictionary<int, string> _parametersNameValueMapper;
        private ParameterInfo[] _methodParameters;
        public ParameterInfo[] MethodParameters
        {
	        set { 
                _methodParameters = value;
                TransformParametersIntoNameValueMapper(_methodParameters);
            }
        }

        private void TransformParametersIntoNameValueMapper(ParameterInfo[] methodParameters)
        {
            _parametersNameValueMapper = new Dictionary<int, string>();
            for (var i = 0; i < methodParameters.Count(); i++)
            {
                _parametersNameValueMapper.Add(i, methodParameters[i].Name);
            }
        }

		public string GetSessionProperty(Arguments arguments)
		{
			return FindSessionProperty(arguments);
		}

		public CacheKeyResult BuildCacheKey(Arguments arguments)
        {
            var cacheKeyBuilder = new StringBuilder();

			var sessionProperty = FindSessionProperty(arguments) ?? "default";
			cacheKeyBuilder.Append(sessionProperty + ".");
			cacheKeyBuilder.Append(MethodName);
            cacheKeyBuilder.Append(".");
			foreach (var arg in arguments)
			{
				AppendToKey(arg, cacheKeyBuilder);
			}
            var code = cacheKeyBuilder.ToString().GetHashCode().ToString();
            return new CacheKeyResult { Key = code, Token = sessionProperty };
        }

		private bool IsSessionProperty(string prop)
		{
			return !string.IsNullOrWhiteSpace(SessionProperty) && string.Compare(prop, SessionProperty, StringComparison.InvariantCultureIgnoreCase) == 0;
		}

		private string FindSessionProperty(IList<object> arguments)
		{
			for (var i = 0; i < _methodParameters.Count(); i++)
			{
                if ( arguments[i] == null )
                {
                    return null;
                }
				if ( IsSessionProperty(_methodParameters[i].Name) && arguments[i] is string) 
				{
					return (string)arguments[i];
				}
				foreach (var pi in arguments[i].GetType().GetProperties().Where(pi => pi.PropertyType == typeof(string) && IsSessionProperty(pi.Name) && pi.CanRead))
				{
					return (string)pi.GetValue(arguments[i], null);
				}
			}
			return null;
		}

        private static void AppendToKey(object o, StringBuilder cacheKeyBuilder)
		{
			if (o == null)
			{
				cacheKeyBuilder.Append("{Null}");
			}
			else if (IsPrimitiveEnough(o))
			{
				cacheKeyBuilder.Append("{");
				cacheKeyBuilder.Append(o);
				cacheKeyBuilder.Append("}");
			}
			else
			{
				cacheKeyBuilder.Append(o.SerializeObject());
			}
		}

		private static bool IsPrimitiveEnough(object o)
		{
			var t = o.GetType();
			return Convert.GetTypeCode(t) != TypeCode.Object;
		}
     
    }

    public class CacheKeyResult
    {
        public string Key { get; set; }
        public string Token { get; set; }
    }

    public enum DeleteSettings
	{
		Token,
        All
	}
}
