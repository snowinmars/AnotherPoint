using AnotherPoint.Common;
using AnotherPoint.Entities;
using AnotherPoint.Entities.MethodImpl;
using AnotherPoint.Extensions;
using AnotherPoint.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace AnotherPoint.Core
{
	public class MethodCore : IMethodCore
	{
		private string[] delete = { "REMOVE", "DELETE" };

		private string[] insert = { "CREATE" };

		private string[] select = { "GET", "READ" };

		private string[] update = { "UPDATE" };

		public void Dispose()
		{
		}

		public Method Map(MethodInfo methodInfo, EntityPurposePair entityPurposePair)
		{
			Method method = new Method(methodInfo.Name, methodInfo.ReturnType.FullName)
			{
				AccessModifyer = this.GetAccessModifyer(methodInfo)
			};

			this.HandleAttributesForBodyGeneration(methodInfo, method.AttributesForBodyGeneration);

			method.EntityPurposePair = entityPurposePair;

			this.HandleArguments(methodInfo.GetParameters(), method.Arguments);

			return method;
		}

		public string RenderAccessModifyer(Method method)
		{
			return method.AccessModifyer.AsString();
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
				body.AppendLine(Constant.MethodBody_ShutUp);
			}

			return body.ToString();
		}

		public string RenderMethodName(Method method)
		{
			return method.Name;
		}

		public string RenderReturnTypeName(Method method)
		{
			if (method.ReturnType.FullName == "System.Void")
			{
				return "void";
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

			string defaultDestination = $"{method.EntityPurposePair.Both.FirstLetterToLower()}Destination"; // TODO

			string destination = sendMeToAttribute.Destination == Constant.DefaultDestination
				? defaultDestination
				: sendMeToAttribute.Destination;

			if (!string.Equals(method.ReturnType.Name, Constant.Void, StringComparison.InvariantCultureIgnoreCase))
			{
				body.Append(" return ");
			}

			body.AppendLine($"this.{destination.FirstLetterToUpper()}.{method.Name}({string.Join(",", method.Arguments.Select(arg => arg.Name))});");

			return body.ToString();
		}

		public string RenderAdditionalBody(Method method)
		{
			return method.AdditionalBody;
		}

		private string GetSqlDeleteCommand(Method method)
		{
			StringBuilder sb = new StringBuilder();

			sb.AppendLine("using (var sqlConnection = new System.Data.SqlClient.SqlConnection(Constant.ConnectionString))");
			sb.AppendLine("{");

			foreach (var argument in method.Arguments)
			{
				sb.Append("sqlConnection.Execute(");

				sb.Append("\"");
				sb.Append(" delete from " +
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

			sb.AppendLine("using (var sqlConnection = new System.Data.SqlClient.SqlConnection(Constant.ConnectionString))");
			sb.AppendLine("{");

			foreach (var argument in method.Arguments)
			{
				Class argumentClass = Bag.ClassPocket[argument.Type.Name];
				IList<Property> properties = argumentClass.Properties.Where(prop => prop.AccessModifyer.HasFlag(AccessModifyer.Public)).ToList();

				sb.Append("sqlConnection.Execute(");

				sb.Append("\"");
				sb.Append(" insert " +
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

			sb.AppendLine("using (var sqlConnection = new System.Data.SqlClient.SqlConnection(Constant.ConnectionString))");
			sb.AppendLine("{");

			foreach (var argument in method.Arguments)
			{
				sb.Append($"return sqlConnection.Query<{method.ReturnType.Name}>(");

				sb.Append("\"");
				sb.Append(" select * from " +
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

			sb.AppendLine("using (var sqlConnection = new System.Data.SqlClient.SqlConnection(Constant.ConnectionString))");
			sb.AppendLine("{");

			foreach (var argument in method.Arguments)
			{
				Class argumentClass = Bag.ClassPocket[argument.Type.Name];
				IList<Property> properties = argumentClass.Properties.Where(prop => prop.AccessModifyer.HasFlag(AccessModifyer.Public)).ToList();

				sb.Append("sqlConnection.Execute(");

				sb.Append("\"");
				sb.Append(" update " +
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
			StringBuilder sb = new StringBuilder();

			string name = method.Name;

			if (this.select.Contains(name.ToUpperInvariant()))
			{
				var toSqlBodyPart = this.GetSqlSelectCommand(method);
				return toSqlBodyPart;
			}

			if (this.insert.Contains(name.ToUpperInvariant()))
			{
				var a = this.GetSqlInsertCommand(method);
				return a;
			}

			if (this.delete.Contains(name.ToUpperInvariant()))
			{
				var a = this.GetSqlDeleteCommand(method);
				return a;
			}

			if (this.update.Contains(name.ToUpperInvariant()))
			{
				var a = this.GetSqlUpdateCommand(method);
				return a;
			}

			return sb.ToString();
		}

		private string GetValidationBodyPart(MethodImpl.ValidateAttribute validateAttribute)
		{
			StringBuilder sb = new StringBuilder();

			foreach (var param in validateAttribute.NamesOfInputParametersToValidate)
			{
				sb.AppendLine($"Validator.Check({param});");
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