using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AnotherPoint.Common;
using AnotherPoint.Entities;
using AnotherPoint.Extensions;

namespace AnotherPoint.Core
{
	public static class MethodCore
	{
		public static string GetBodyAsString(Method method)
		{
			return "";
		}

		public static Method Map(MethodInfo methodInfo)
		{
			Method method = new Method(methodInfo.Name, methodInfo.ReturnType.FullName)
			{
				AccessModifyer = MethodCore.GetAccessModifyer(methodInfo)
			};

			if (method.ReturnType.Name != Constant.Void)
			{
				HandleArguments(method, methodInfo);
			}

			return method;
		}

		public static string GetArgumentsAsString(Method method)
		{
			if (method.ReturnType.Name == Constant.Void)
			{
				return "";
			}

			StringBuilder sb = new StringBuilder();

			foreach (var argument in method.Arguments)
			{
				sb.Append($"{argument.Type.FullName} {argument.Name.FirstLetterToLower()},");
			}

			sb.Remove(sb.Length - 1, 1);

			return sb.ToString();
		}

		private static void HandleArguments(Method method, MethodInfo methodInfo)
		{
			foreach (var parameterInfo in methodInfo.GetParameters())
			{
				Argument argument = new Argument(parameterInfo.Name, parameterInfo.ParameterType.FullName, BindSettings.None);

				method.Arguments.Add(argument);
			}
		}

		private static AccessModifyer GetAccessModifyer(MethodInfo methodInfo)
		{
			AccessModifyer accessModifyer = AccessModifyer.None;

			if (methodInfo.IsPublic)
			{
				accessModifyer |= AccessModifyer.Public;
			}

			if (methodInfo.IsInternal())
			{
				accessModifyer |= AccessModifyer.Internal;
			}

			if (methodInfo.IsProtected())
			{
				accessModifyer |= AccessModifyer.Protected;
			}

			if (methodInfo.IsProtectedInternal())
			{
				accessModifyer |= AccessModifyer.Internal | AccessModifyer.Protected;
			}

			if (methodInfo.IsPrivate)
			{
				accessModifyer |= AccessModifyer.Private;
			}

			if (methodInfo.IsAbstract)
			{
				accessModifyer |= AccessModifyer.Abstract;
			}

			if (methodInfo.IsVirtual)
			{
				accessModifyer |= AccessModifyer.Virtual;
			}

			return accessModifyer;
		}
	}
}
