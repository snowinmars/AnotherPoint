using AnotherPoint.Common;
using AnotherPoint.Entities;
using AnotherPoint.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using AnotherPoint.Interfaces;

namespace AnotherPoint.Core
{
	public class CtorCore : ICtorCore
	{
		public string RenderAccessModifyer(Ctor ctor)
		{
			return ctor.AccessModifyer.AsString();
		}

		public string RenderArgumentCollection(Ctor ctor)
		{
			IList<KeyValuePair<string, string>> parameters =
				(
					from argument
					in ctor.ArgumentCollection
						.Where(arg => arg.BindAttribute == BindSettings.Exact ||
										arg.BindAttribute == BindSettings.None)
					let type = argument.GetFullTypeName()
					let parameter = argument.Name.FirstLetterToLower()
					select
						new KeyValuePair<string, string>(type, parameter)
				 ).ToList();

			return MergeParametersCollectionToString(parameters);
		}

		public string RenderBody(Ctor ctor)
		{
			StringBuilder body = new StringBuilder(256);

			foreach (var bind in ctor.ArgumentCollection)
			{
				switch (bind.BindAttribute)
				{
					case BindSettings.Exact:
						body.Append(GetExactBindingArgumentString(bind));
						break;

					case BindSettings.New:
						body.Append(GetNewBindingArgumentString(bind));
						break;

					case BindSettings.CallThis:
						// do nothing here: I handle it in GetCtorCarriage()
						break;

					case BindSettings.None:
						break;

					default:
						throw new ArgumentOutOfRangeException(nameof(bind), bind, $"Enum {nameof(BindSettings)} is out of range");
				}

				body.AppendLine();
			}

			return body.ToString();
		}

		public string RenderCtorCarriage(Ctor ctor)
		{
			StringBuilder carriage = new StringBuilder(256);

			foreach (var bind in ctor.ArgumentCollection)
			{
				switch (bind.BindAttribute)
				{
					case BindSettings.CallThis:
						carriage.Append($"{bind.Name.FirstLetterToLower()},");
						break;

					case BindSettings.Exact:
						// do nothing here: I handle it in GetBodyAsString()
						break;

					case BindSettings.New:
						// do nothing here: I handle it in GetBodyAsString()
						break;

					case BindSettings.None:
						break;

					default:
						throw new ArgumentOutOfRangeException(nameof(bind), bind, $"Enum {nameof(BindSettings)} is out of range");
				}
			}

			if (carriage.Length > 0)
			{
				carriage.RemoveLastSymbol();
				carriage.Insert(0, " : this(");
				carriage.Append(")");
			}

			return carriage.ToString();
		}

		public string RenderTypeName(Ctor ctor)
		{
			return ctor.Type.Name;
		}

		public Ctor Map(ConstructorInfo constructorInfo)
		{
			Type declaringType = constructorInfo.DeclaringType;

			if (declaringType == null)
			{
				throw new ArgumentException($"Declaring type of constructor {constructorInfo.Name} is null, wtf");
			}

			Ctor ctor = new Ctor(declaringType.FullName)
			{
				AccessModifyer = GetAccessModifyer(constructorInfo)
			};

			HandleCtorArguments(constructorInfo, ctor);

			return ctor;
		}

		private AccessModifyer GetAccessModifyer(ConstructorInfo constructorInfo)
		{
			AccessModifyer accessModifyer = AccessModifyer.None;

			if (constructorInfo.IsPublic)
			{
				accessModifyer |= AccessModifyer.Public;
			}

			if (constructorInfo.IsProtected())
			{
				accessModifyer |= AccessModifyer.Protected;
			}

			if (constructorInfo.IsPrivate)
			{
				accessModifyer |= AccessModifyer.Private;
			}

			if (constructorInfo.IsAbstract)
			{
				accessModifyer |= AccessModifyer.Abstract;
			}

			return accessModifyer;
		}

		private string GetExactBindingArgumentString(Argument bind)
		{
			StringBuilder sb = new StringBuilder();

			sb.Append(" this. ");
			sb.Append(bind.Name.FirstLetterToUpper());
			sb.Append(" = ");
			sb.Append(bind.Name.FirstLetterToLower());
			sb.Append(";");

			return sb.ToString();
		}

		private string GetFullTypeNameWithoutAssmblyInfo(MyType type)
		{
			return type.FullName
				.Split(new[] { '[' }, StringSplitOptions.RemoveEmptyEntries)
				.First();
		}

		private void GetGenericTypesAsString(StringBuilder sb, MyType type)
		{
			sb.Append("<");
			sb.Append(string.Join(",", type.GenericTypes));
			sb.Append(">");
		}

		private string GetNewBindingArgumentString(Argument bind)
		{
			StringBuilder sb = new StringBuilder(256);

			sb.Append(" this. ");
			sb.Append(bind.Name.FirstLetterToUpper());
			sb.Append(" = ");
			sb.Append(" new ");

			MyType type = Bag.Pocket[bind.Name.ToUpperInvariant()];

			string fullTypeNameWithoutAssmblyInfo = type.GetFullNameWithoutAssemblyInfo(type.FullName);
			string fullImplementTypeName = Helpers.GetImplementTypeNaming(fullTypeNameWithoutAssmblyInfo);
			sb.Append(fullImplementTypeName);

			if (type.IsGeneric.HasValue &&
				type.IsGeneric.Value)
			{
				GetGenericTypesAsString(sb, type);
			}

			sb.Append("();");

			return sb.ToString();
		}

		private void HandleCtorArguments(ConstructorInfo constructorInfo, Ctor ctor)
		{
			foreach (var ctorBind in constructorInfo.GetCustomAttributes<BindAttribute>())
			{
				if (ctorBind.Settings == BindSettings.CallThis)
				{
					Argument arg = new Argument(ctorBind.Name, "System", BindSettings.CallThis);

					ctor.ArgumentCollection.Add(arg);
				}
				else
				{
					MyType argType = Bag.Pocket[ctorBind.Name.ToUpperInvariant()];
					Argument arg = new Argument(ctorBind.Name, argType.FullName, ctorBind.Settings)
					{
						Type = argType
					};

					ctor.ArgumentCollection.Add(arg);
				}
			}
		}

		private string MergeParametersCollectionToString(IList<KeyValuePair<string, string>> parameters)
		{
			StringBuilder args = new StringBuilder(128);

			foreach (var parameter in parameters)
			{
				args.Append($"{parameter.Key} {parameter.Value},");
			}

			if (args.Length > 0)
			{
				args.RemoveLastSymbol();
			}

			return args.ToString();
		}

		public void Dispose()
		{
		}
	}
}