using System;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Core.Contract;
using Utilities;

namespace Core.Infrastructure {

	public class ReflectionHelper {

		public static AppDomain LoadAppDomain(string folder, string assemblyFile, string configFile = null) {
			string assemblyPath = Path.Combine(folder, assemblyFile);
			string configurationFile = configFile ?? assemblyPath + ".config";
			AppDomainSetup setupInfo = new AppDomainSetup { ApplicationBase = folder, ConfigurationFile = configurationFile };
			Logger.Debug("Loading assembly " + assemblyPath);
			AppDomain appDomain = AppDomain.CreateDomain(Path.GetFileNameWithoutExtension(assemblyPath), null, setupInfo);
			return appDomain;
		}

		public static TInstance CreateSponsoredInstance<TInstance>(AppDomain appDomain, Type type, params object[] parameters) where TInstance : MarshalByRefObject {
			TInstance instance = CreateInstance<TInstance>(appDomain, type, parameters);
			RemoteObjectFactory.Sponsor(instance);
			return (instance);
		}

		public static TInstance CreateInstance<TInstance>(AppDomain appDomain, Type type, params object[] parameters) where TInstance : class {
			string assemblyFile = type.Assembly.File;
			if (assemblyFile == null) throw new ArgumentException("Type must include assembly location");
			string assemblyName = type.Assembly.Name;
			object proxy = appDomain.CreateInstanceAndUnwrap(assemblyName, type.FullName, false, BindingFlags.CreateInstance, null, parameters, null, null);
			TInstance instance = proxy as TInstance;
			if(instance == null) throw new Exception("Type " + type.FullName + " cannot be cast to " + typeof(TInstance).FullName);
			Logger.Info("Created instance of " + type.FullName + " from " + assemblyFile + " in app domain " + appDomain.FriendlyName);
			return (instance);
		}

		public static PropertyInfo GetProperty<T>(Expression<Func<T, object>> selection) where T : class {
			MemberExpression e = GetMember(selection);
			if(e.Member.MemberType == MemberTypes.Property) {
				PropertyInfo f = (PropertyInfo)e.Member;
				return (f);
			}
			throw new ArgumentException("That is not a property");
		}

		public static MethodInfo GetMethod<T>(Expression<Func<T, object>> selection) where T : class {
			MemberExpression e = GetMember(selection);
			if(e.Member.MemberType == MemberTypes.Method) {
				MethodInfo f = (MethodInfo)e.Member;
				return (f);
			}
			throw new ArgumentException("That is not a method");
		}

		public static MethodInfo GetMethodInfo<T>(Expression<Func<T, Delegate>> expression) {
			var unaryExpression = (UnaryExpression)expression.Body;
			var methodCallExpression = (MethodCallExpression)unaryExpression.Operand;
			var methodInfoExpression = (ConstantExpression)methodCallExpression.Arguments.Last();
			var methodInfo = (MethodInfo)methodInfoExpression.Value;
			return methodInfo;
		}

		private static MemberExpression GetMember(Expression member) {
			LambdaExpression lambda = member as LambdaExpression;
			if(lambda == null) throw new ArgumentNullException("method");
			MemberExpression memberExpr = null;
			if(lambda.Body.NodeType == ExpressionType.Convert) {
				memberExpr = ((UnaryExpression)lambda.Body).Operand as MemberExpression;
			} else if(lambda.Body.NodeType == ExpressionType.MemberAccess) {
				memberExpr = lambda.Body as MemberExpression;
			}
			if(memberExpr == null) throw new ArgumentException("method");
			return memberExpr;
		}

		
	}

	public static class SymbolExtensions {
		/// <summary>
		/// Given a lambda expression that calls a method, returns the method info.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="expression">The expression.</param>
		/// <returns></returns>
		public static MethodInfo GetMethodInfo(Expression<Action> expression) {
			return GetMethodInfo((LambdaExpression)expression);
		}

		/// <summary>
		/// Given a lambda expression that calls a method, returns the method info.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="expression">The expression.</param>
		/// <returns></returns>
		public static MethodInfo GetMethodInfo<T>(Expression<Action<T>> expression) {
			return GetMethodInfo((LambdaExpression)expression);
		}

		/// <summary>
		/// Given a lambda expression that calls a method, returns the method info.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="expression">The expression.</param>
		/// <returns></returns>
		public static MethodInfo GetMethodInfo<T, TResult>(Expression<Func<T, TResult>> expression) {
			return GetMethodInfo((LambdaExpression)expression);
		}

		/// <summary>
		/// Given a lambda expression that calls a method, returns the method info.
		/// </summary>
		/// <param name="expression">The expression.</param>
		/// <returns></returns>
		public static MethodInfo GetMethodInfo(LambdaExpression expression) {
			MethodCallExpression outermostExpression = expression.Body as MethodCallExpression;

			if(outermostExpression == null) {
				throw new ArgumentException("Invalid Expression. Expression should consist of a Method call only.");
			}

			return outermostExpression.Method;
		}
	}


}