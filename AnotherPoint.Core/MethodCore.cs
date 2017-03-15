using AnotherPoint.Common;
using AnotherPoint.Entities;
using AnotherPoint.Entities.MethodImpl;
using AnotherPoint.Extensions;
using AnotherPoint.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

namespace AnotherPoint.Core
{
	public class MethodCore : IMethodCore
	{
		private readonly string[] deleteSynonyms = { "REMOVE", "DELETE" };

		private readonly string[] insertSynonyms = { "CREATE" };

		private readonly string[] selectSynonyms = { "GET", "READ" };

		private readonly string[] updateSynonyms = { "UPDATE" };

		public void Dispose()
		{
		}

		public Method Map(MethodInfo methodInfo, EntityPurposePair entityPurposePair)
		{
			Log.Info($"Mapping method {methodInfo.ReturnType.FullName} {methodInfo.Name}(?)...");

			Stopwatch sw = Stopwatch.StartNew();
			
			Method method = new Method(methodInfo.Name, methodInfo.ReturnType.FullName)
			{
				AccessModifyer = this.GetAccessModifyer(methodInfo)
			};

			this.HandleAttributesForBodyGeneration(methodInfo, method.AttributesForBodyGeneration);

			method.EntityPurposePair = entityPurposePair;

			this.HandleArguments(methodInfo.GetParameters(), method.Arguments);

			sw.Stop();

			Log.iDone(sw.Elapsed.TotalMilliseconds);

			return method;
		}

		public string RenderAccessModifyer(Method method)
		{
			return method.AccessModifyer.AsString();
		}

		public string RenderAdditionalBody(Method method)
		{
			return method.AdditionalBody;
		}

		public string RenderArguments(Method method)
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

		public string RenderBody(Method method)
		{
			MethodImpl.ShutMeUpAttribute shutMeUpAttribute = null;
			MethodImpl.SendMeToAttribute sendMeToAttribute = null;
			MethodImpl.ValidateAttribute validateAttribute = null;
			MethodImpl.ToSqlAttribute toSqlAttribute = null;

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

				if (toSqlAttribute == null)
				{
					toSqlAttribute = attribute as MethodImpl.ToSqlAttribute;
				}
			}

			StringBuilder body = new StringBuilder();

			if (validateAttribute != null)
			{
				body.AppendLine(this.GetValidationBodyPart(validateAttribute));
			}

			if (toSqlAttribute != null)
			{
				body.AppendLine(this.GetToSqlBodyPart(method));
			}

			if (sendMeToAttribute != null)
			{
				body.AppendLine(this.GetSendMeToBodyPart(method, sendMeToAttribute));
			}

			if (shutMeUpAttribute != null)
			{
				body.AppendLine(Constant.MethodBodyShutUp);
			}

			return body.ToString();
		}

		public string RenderMethodName(Method method)
		{
			return method.Name;
		}

		public string RenderReturnTypeName(Method method)
		{
			if (method.ReturnType.FullName == Constant.Types.SystemVoid)
			{
				return Constant.Void;
			}

			return method.ReturnType.FullName;
		}

		private AccessModifyer GetAccessModifyer(MethodInfo methodInfo)
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

		private string GetSendMeToBodyPart(Method method, MethodImpl.SendMeToAttribute sendMeToAttribute)
		{
			StringBuilder body = new StringBuilder();

			string defaultDestination = Helpers.GetDefaultDestinationName(sendMeToAttribute.From);

			string destination = sendMeToAttribute.Destination == Constant.DefaultDestination
				? defaultDestination
				: sendMeToAttribute.Destination;

			if (!string.Equals(method.ReturnType.FullName, Constant.Types.SystemVoid, StringComparison.InvariantCultureIgnoreCase))
			{
				body.Append($" {Constant.Return} ");
			}

			body.AppendLine($"{Constant.This}.{destination.FirstLetterToUpper()}.{method.Name}({string.Join(",", method.Arguments.Select(arg => arg.Name))});");

			return body.ToString();
		}

		private string GetSqlDeleteCommand(Method method)
		{
			StringBuilder sb = new StringBuilder();

			sb.AppendLine($"using (var sqlConnection = new {Constant.Types.SystemDataSqlClientSqlConnection}({Constant._Constant}.{Constant.ConnectionString}))");
			sb.AppendLine("{");

			foreach (var argument in method.Arguments)
			{
				sb.Append("sqlConnection.Execute(");

				sb.Append("\"");
				sb.Append(" deleteSynonyms from " +
						  $" [{method.EntityPurposePair}s] " + // table name in plural
						  " where " +
						  $" {argument.Name.FirstLetterToUpper()} = @{argument.Name.FirstLetterToLower()} ");
				sb.Append("\"");

				sb.Append(", param: new {");

				sb.Append(argument.Name.FirstLetterToLower());

				sb.AppendLine("});");
			}

			sb.AppendLine("}");

			return sb.ToString();
		}

		private string GetSqlInsertCommand(Method method)
		{
			StringBuilder sb = new StringBuilder();

			sb.AppendLine($"using (var sqlConnection = new {Constant.Types.SystemDataSqlClientSqlConnection}({Constant._Constant}.{Constant.ConnectionString}))");
			sb.AppendLine("{");

			foreach (var argument in method.Arguments)
			{
				Class argumentClass = Bag.ClassPocket[argument.Type.Name];
				IList<Property> properties = argumentClass.Properties.Where(prop => prop.AccessModifyer.HasFlag(AccessModifyer.Public)).ToList();

				sb.Append("sqlConnection.Execute(");

				sb.Append("\"");
				sb.Append(" insertSynonyms " +
						  $" [{argumentClass.Name}s] " + // table name in plural
						  $" ({string.Join(",", properties.Select(prop => prop.Name.FirstLetterToUpper()))}) " + // columns' names
						  " values" +
						  $" ({string.Join(",", properties.Select(prop => $"@{prop.Name.FirstLetterToLower()}"))}) "); // values with @ prefix
				sb.Append("\"");

				sb.Append(", param: new {");

				if (properties.Any())
				{
					sb.Append(string.Join(",", properties.Select(prop => $"{argument.Name}.{prop.Name}")));
				}

				sb.AppendLine("});");
			}

			sb.AppendLine("}");

			return sb.ToString();
		}

		private string GetSqlSelectCommand(Method method)
		{
			StringBuilder sb = new StringBuilder();

			sb.AppendLine($"using (var sqlConnection = new {Constant.Types.SystemDataSqlClientSqlConnection}({Constant._Constant}.{Constant.ConnectionString}))");
			sb.AppendLine("{");

			foreach (var argument in method.Arguments)
			{
				sb.Append($"return sqlConnection.Query<{method.ReturnType.FullName}>(");

				sb.Append("\"");
				sb.Append(" selectSynonyms * from " +
						  $" [{method.EntityPurposePair}s] " + // table name in plural
						  " where" +
						  " (Id = @id) "); // values with @ prefix
				sb.Append("\"");

				sb.Append(", param: new {");

				sb.Append(argument.Name.FirstLetterToLower());

				sb.AppendLine("}).FirstOrDefault();");
			}

			sb.AppendLine("}");

			return sb.ToString();
		}

		private string GetSqlUpdateCommand(Method method)
		{
			StringBuilder sb = new StringBuilder();

			sb.AppendLine($"using (var sqlConnection = new {Constant.Types.SystemDataSqlClientSqlConnection}({Constant._Constant}.{Constant.ConnectionString}))");
			sb.AppendLine("{");

			foreach (var argument in method.Arguments)
			{
				Class argumentClass = Bag.ClassPocket[argument.Type.Name];
				IList<Property> properties = argumentClass.Properties.Where(prop => prop.AccessModifyer.HasFlag(AccessModifyer.Public)).ToList();

				sb.Append("sqlConnection.Execute(");

				sb.Append("\"");
				sb.Append(" updateSynonyms " +
						  $" [{argumentClass.Name}s] " + // table name in plural
						  " set " +
						  $" ({string.Join(",", properties.Select(prop => $"{prop.Name.FirstLetterToUpper()} = @{prop.Name.FirstLetterToLower()}"))}) " + // columns' names
						  " where" +
						  " (Id = @id) "); // values with @ prefix
				sb.Append("\"");

				sb.Append(", param: new {");

				if (properties.Any())
				{
					sb.Append(string.Join(",", properties.Select(prop => $"{argument.Name}.{prop.Name}")));
				}

				sb.AppendLine("});");
			}

			sb.AppendLine("}");

			return sb.ToString();
		}

		private string GetToSqlBodyPart(Method method)
		{
			string name = method.Name.ToUpperInvariant();

			if (this.selectSynonyms.Contains(name))
			{
				return this.GetSqlSelectCommand(method);
			}

			if (this.insertSynonyms.Contains(name))
			{
				return this.GetSqlInsertCommand(method);
			}

			if (this.deleteSynonyms.Contains(name))
			{
				return this.GetSqlDeleteCommand(method);
			}

			if (this.updateSynonyms.Contains(name))
			{
				return this.GetSqlUpdateCommand(method);
			}

			return "";
		}

		private string GetValidationBodyPart(MethodImpl.ValidateAttribute validateAttribute)
		{
			StringBuilder sb = new StringBuilder();

			foreach (var param in validateAttribute.NamesOfInputParametersToValidate)
			{
				sb.AppendLine($"{Constant.Validation}.Check({param});");
			}

			return sb.ToString();
		}

		private void HandleArguments(IEnumerable<ParameterInfo> systemTypeParameters, ICollection<Argument> methodArguments)
		{
			foreach (var parameterInfo in systemTypeParameters)
			{
				Argument argument = new Argument(parameterInfo.Name, parameterInfo.ParameterType.FullName, BindSettings.None);

				methodArguments.Add(argument);
			}
		}

		private void HandleAttributesForBodyGeneration(MethodInfo methodInfo, ICollection<Attribute> methodAttributes)
		{
			foreach (var methodImplAttribute in methodInfo.GetCustomAttributes<MethodImpl.ShutMeUpAttribute>())
			{
				methodAttributes.Add(methodImplAttribute);
			}

			foreach (var toSqlAttribute in methodInfo.GetCustomAttributes<MethodImpl.ToSqlAttribute>())
			{
				methodAttributes.Add(toSqlAttribute);
			}

			foreach (var methodImplAttribute in methodInfo.GetCustomAttributes<MethodImpl.SendMeToAttribute>())
			{
				methodAttributes.Add(methodImplAttribute);
			}

			foreach (var methodImplAttribute in methodInfo.GetCustomAttributes<MethodImpl.ValidateAttribute>())
			{
				methodAttributes.Add(methodImplAttribute);
			}
		}
	}
}