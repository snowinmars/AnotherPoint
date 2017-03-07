using AnotherPoint.Common;
using AnotherPoint.Entities;
using AnotherPoint.Extensions;
using AnotherPoint.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

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
						body.Append(GetExactBindingArgumentString(bind.Name));
						break;

					case BindSettings.New:
						body.Append(GetNewBindingArgumentString(bind.Name));
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

			HandleCtorArguments(constructorInfo, ctor.ArgumentCollection);

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

		private string GetExactBindingArgumentString(string bindName)
		{
			StringBuilder sb = new StringBuilder();

			sb.Append(" this. ");
			sb.Append(bindName.FirstLetterToUpper());
			sb.Append(" = ");
			sb.Append(bindName.FirstLetterToLower());
			sb.Append(";");

			return sb.ToString();
		}

		private string GetGenericTypesAsString(IEnumerable<string> genericTypes)
		{
			StringBuilder sb = new StringBuilder();

			sb.Append("<");
			sb.Append(string.Join(",", genericTypes));
			sb.Append(">");

			return sb.ToString();
		}

		private string GetNewBindingArgumentString(string bingName)
		{
			StringBuilder sb = new StringBuilder(256);

			sb.Append(" this. ");
			sb.Append(bingName.FirstLetterToUpper());
			sb.Append(" = ");
			sb.Append(" new ");

			MyType type = Bag.TypePocket[bingName.ToUpperInvariant()];

			string fullTypeNameWithoutAssmblyInfo = type.GetFullNameWithoutAssemblyInfo(type.FullName);
			string fullImplementTypeName = Helpers.GetImplementTypeNaming(fullTypeNameWithoutAssmblyInfo);
			sb.Append(fullImplementTypeName);

			if (type.IsGeneric.HasValue &&
				type.IsGeneric.Value)
			{
				sb.Append(GetGenericTypesAsString(type.GenericTypes));
			}

			sb.Append("();");

			return sb.ToString();
		}

		private void HandleCtorArguments(ConstructorInfo constructorInfo, IList<Argument> ctorArguments)
		{
			foreach (var ctorBind in constructorInfo.GetCustomAttributes<BindAttribute>())
			{
				if (ctorBind.Settings == BindSettings.CallThis)
				{
					Argument arg = new Argument(ctorBind.Name, "System", BindSettings.CallThis);

					ctorArguments.Add(arg);
				}
				else
				{
					MyType argType = Bag.TypePocket[ctorBind.Name.ToUpperInvariant()];
					Argument arg = new Argument(ctorBind.Name, argType.FullName, ctorBind.Settings)
					{
						Type = argType
					};

					ctorArguments.Add(arg);
				}
			}
		}

		private string MergeParametersCollectionToString(IEnumerable<KeyValuePair<string, string>> parameters)
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