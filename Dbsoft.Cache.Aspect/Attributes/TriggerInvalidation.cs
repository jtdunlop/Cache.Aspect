// Heavily modified version of Postsharp.Cache https://www.nuget.org/packages/PostSharp.Cache

namespace DbSoft.Cache.Aspect.Attributes
{
    using System;
    using System.Diagnostics;
    using System.Reflection;
    using PostSharp.Aspects;
    using Supporting;

    public static partial class Cache
    {
        [Serializable]
        public class TriggerInvalidation : OnMethodBoundaryAspect
        {
            private KeyBuilder _keyBuilder;
			private readonly DeleteSettings _settings;

	        private KeyBuilder KeyBuilder => _keyBuilder ?? (_keyBuilder = new KeyBuilder());

            public TriggerInvalidation(DeleteSettings settings)
            {
				_settings = settings;
            }

	        private TriggerInvalidation()
                : this(DeleteSettings.All)
            {
            }
 
            //Method executed at build time.
            public override void CompileTimeInitialize(MethodBase method, AspectInfo aspectInfo)
            {
                KeyBuilder.MethodParameters = method.GetParameters();
	            Debug.Assert(method.DeclaringType != null, "method.DeclaringType != null");
	            KeyBuilder.MethodName = $"{method.DeclaringType.FullName}.{method.Name}";
            }

            public override void OnExit(MethodExecutionArgs args)
            {
				KeyBuilder.SessionProperty = CacheService.SessionProperty;
	            switch (_settings)
	            {
		            case DeleteSettings.Token:
			            CacheService.GetCache("default").DeleteAll(KeyBuilder.GetSessionProperty(args.Arguments) + ".");
			            break;
                    case DeleteSettings.All:
                        CacheService.GetCache("default").DeleteAll();
                        break;
	            }
	            base.OnExit(args);
            }
        }
    }
}

