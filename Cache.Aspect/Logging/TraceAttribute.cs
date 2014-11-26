// Heavily modified version of Postsharp.Cache https://www.nuget.org/packages/PostSharp.Cache

namespace DbSoft.Cache.Aspect.Logging
{
    using System;
    using System.Diagnostics;
    using System.Reflection;
    using System.Text;
    using PostSharp.Aspects;

    [Serializable]
	public class TraceAttribute : OnMethodBoundaryAspect
	{
		private string _methodName;
		private DateTime _start;

		private double MinimumExecutionTimeToTrace { get; set; }

		public TraceAttribute(double minimumExecutionTimeToLog = .25)
		{
			MinimumExecutionTimeToTrace = minimumExecutionTimeToLog;
		}

		/// <summary> 
		/// Method executed at build time. Initializes the aspect instance. After the execution 
		/// of <see cref="CompileTimeInitialize"/>, the aspect is serialized as a managed  
		/// resource inside the transformed assembly, and deserialized at runtime. 
		/// </summary> 
		/// <param name="method">Method to which the current aspect instance  
		/// has been applied.</param> 
		/// <param name="aspectInfo">Unused.</param> 
		public override void CompileTimeInitialize(MethodBase method, AspectInfo aspectInfo)
		{
			Debug.Assert(method.DeclaringType != null, "method.DeclaringType != null");
			_methodName = method.DeclaringType.FullName + "." + method.Name;
		}

		/// <summary> 
		/// Method invoked before the execution of the method to which the current 
		/// aspect is applied. 
		/// </summary> 
		/// <param name="args">Unused.</param> 
		public override void OnEntry(MethodExecutionArgs args)
		{
			_start = DateTime.Now;
		}

		/// <summary> 
		/// Method invoked after successfull execution of the method to which the current 
		/// aspect is applied. 
		/// </summary> 
		/// <param name="args">Unused.</param> 
		public override void OnSuccess(MethodExecutionArgs args)
		{
		}

		/// <summary> 
		/// Method invoked after failure of the method to which the current 
		/// aspect is applied. 
		/// </summary> 
		/// <param name="args">Unused.</param> 
		public override void OnException(MethodExecutionArgs args)
		{
			var stringBuilder = new StringBuilder(1024);

			// Write the exit message. 
			stringBuilder.Append(_methodName);
			stringBuilder.Append('(');

			// Write the current instance object, unless the method 
			// is static. 
			object instance = args.Instance;
			if (instance != null)
			{
				stringBuilder.Append("this=");
				stringBuilder.Append(instance);
				if (args.Arguments.Count > 0)
					stringBuilder.Append("; ");
			}

			// Write the list of all arguments. 
			for (int i = 0; i < args.Arguments.Count; i++)
			{
				if (i > 0)
					stringBuilder.Append(", ");
				stringBuilder.Append(args.Arguments.GetArgument(i) ?? "null");
			}

			// Write the exception message. 
			stringBuilder.AppendFormat("): Exception ");
			stringBuilder.Append(args.Exception.GetType().Name);
			stringBuilder.Append(": ");
			stringBuilder.Append(args.Exception.Message);
		}
	}
}
