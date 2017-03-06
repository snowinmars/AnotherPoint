using AnotherPoint.Common;
using AnotherPoint.Entities;
using AnotherPoint.Entities.MethodImpl;
using AnotherPoint.Extensions;
using System;
using System.Linq;
using System.Reflection;
using System.Text;

namespace AnotherPoint.Core
{
	public static class MethodCore
	{
		public static string GetArgumentsAsString(Method method)
		{
			StringBuilder sb = new StringBuilder();

			foreach (var argument in method.Arguments)
			{
				sb.Append($"{argument.Type.FullName} {argument.Name.FirstLetterToLower()},");
			}

			if (sb.Length > 0)
			{
				sb.RemoveLastSymbol();
			}

			return sb.ToString();
		}

		public static string GetBodyAsString(Method method)
		{
			MethodImpl.ShutMeUpAttribute shutMeUpAttribute = null;
			MethodImpl.SendMeToAttribute sendMeToAttribute = null;
			MethodImpl.ValidateAttribute validateAttribute = null;

			if (method.AttributesForBodyGeneration.Count > 3)
			{
				throw new InvalidOperationException("This can't be");
			}

			foreach (var attribute in method.AttributesForBodyGeneration)
			{
				if (shutMeUpAttribute == null)
				{
					shutMeUpAttribute = attribute as MethodImpl.ShutMeUpAttribute;
				}

				if (sendMeToAttribute == null)
				{
					sendMeToAttribute = attribute as MethodImpl.SendMeToAttribute;
				}

				if (validateAttribute == null)
				{
					validateAttribute = attribute as MethodImpl.ValidateAttribute;
				}
			}

			StringBuilder body = new StringBuilder();

			if (validateAttribute != null)
			{
				body.AppendLine(MethodCore.GetValidationBodyPart(validateAttribute));
			}

			if (sendMeToAttribute != null)
			{
				body.AppendLine(MethodCore.GetSendMeToBodyPart(method, sendMeToAttribute));
			}

			if (shutMeUpAttribute != null)
			{
				body.AppendLine(Constant.MethodBody_ShutUp);
			}

			return body.ToString();
		}

		public static Method Map(MethodInfo methodInfo, string className = null)
		{
			Method method = new Method(methodInfo.Name, methodInfo.ReturnType.FullName)
			{
				AccessModifyer = MethodCore.GetAccessModifyer(methodInfo)
			};

			MethodCore.HandleAttributesForBodyGeneration(methodInfo, method);

			method.ForClass = className;

			MethodCore.HandleArguments(method, methodInfo);

			return method;
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

		private static string GetSendMeToBodyPart(Method method, MethodImpl.SendMeToAttribute sendMeToAttribute)
		{
			StringBuilder body = new StringBuilder();

			string defaultDestination = $"{method.ForClass.FirstLetterToLower()}Destination"; // TODO

			string destination = sendMeToAttribute.Destination == Constant.DefaultDestination
				? defaultDestination
				: sendMeToAttribute.Destination;

			if (!string.Equals(method.ReturnType.Name, Constant.Void, StringComparison.InvariantCultureIgnoreCase))
			{
				body.Append(" return ");
			}

			body.AppendLine($"this.{destination.FirstLetterToLower()}.{method.Name}({string.Join(",", method.Arguments.Select(arg => arg.Name))});");

			return body.ToString();
		}

		private static string GetValidationBodyPart(MethodImpl.ValidateAttribute validateAttribute)
		{
			StringBuilder sb = new StringBuilder();

			foreach (var param in validateAttribute.NamesOfInputParametersToValidate)
			{
				sb.AppendLine($"Validator.Check({param});");
			}

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

		private static void HandleAttributesForBodyGeneration(MethodInfo methodInfo, Method method)
		{
			foreach (var methodImplAttribute in methodInfo.GetCustomAttributes<MethodImpl.ShutMeUpAttribute>())
			{
				method.AttributesForBodyGeneration.Add(methodImplAttribute);
			}

			foreach (var methodImplAttribute in methodInfo.GetCustomAttributes<MethodImpl.SendMeToAttribute>())
			{
				method.AttributesForBodyGeneration.Add(methodImplAttribute);
			}

			foreach (var methodImplAttribute in methodInfo.GetCustomAttributes<MethodImpl.ValidateAttribute>())
			{
				method.AttributesForBodyGeneration.Add(methodImplAttribute);
			}
		}
	}
}