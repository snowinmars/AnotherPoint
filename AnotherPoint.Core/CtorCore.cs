using AnotherPoint.Common;
using AnotherPoint.Entities;
using AnotherPoint.Extensions;
using System;
using System.Linq;
using System.Reflection;
using System.Text;

namespace AnotherPoint.Core
{
	public static class CtorCore
	{
		public static string GetArgumentCollectionAsString(Ctor ctor)
		{
			StringBuilder args = new StringBuilder(128);

			foreach (var parameter in ctor.ArgumentCollection)
			{
				args.Append(parameter.Type.FullName);

				if (parameter.Type.IsGeneric)
				{
					args.Append("<");

					string s = string.Join(",", parameter.Type.GenericTypes);
					args.Append(s);

					args.Append(">");
				}

				args.Append(" ");
				args.Append(parameter.Name.FirstLetterToLower());
				args.Append(", ");
			}

			if (args.Length > 0)
			{
				args.Remove(args.Length - 2, 1); // removing last comma
			}

			return args.ToString();
		}

		public static string GetBodyAsString(Ctor ctor)
		{
			StringBuilder body = new StringBuilder(256);

			foreach (var bind in ctor.ArgumentCollection)
			{
				switch (bind.BindAttribute)
				{
					case CtorBindSettings.Exact:
						body.Append(" this. ");
						body.Append(bind.Name.FirstLetterToUpper());
						body.Append(" = ");
						body.Append(bind.Name.FirstLetterToLower());
						body.Append(";");
						break;

					case CtorBindSettings.New:
						body.Append(" this. ");
						body.Append(bind.Name.FirstLetterToUpper());
						body.Append(" = ");
						body.Append(" new ");

						string typeFullName = Bag.Pocket[bind.Name];

						var fullTypeNameWithoutAssmblyInfo = typeFullName.Split(new[] { '[' }, StringSplitOptions.RemoveEmptyEntries).First();
						var typeName = Helpers.GetCorrectCollectionTypeNaming(fullTypeNameWithoutAssmblyInfo.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries).Last());
						var fullImplementTypeName = Helpers.GetImplementTypeNaming(typeName);
						body.Append(fullImplementTypeName);

						if (Helpers.GetImplementTypeNaming(fullTypeNameWithoutAssmblyInfo).Contains(Constant.Generic))
						{
							var v = typeFullName.IndexOf("<");

							if (v >= 0)
							{
								body.Append(typeFullName.Substring(v));
							}
						}
						body.Append("();");
						break;

					case CtorBindSettings.None:
						throw new ArgumentException($"Enum {nameof(CtorBindSettings)} can't equals to None");
					default:
						throw new ArgumentOutOfRangeException("bind", bind, "Enum CtorBindSettings is out of range");
				}

				body.AppendLine();
			}

			return body.ToString();
		}

		public static Ctor Map(ConstructorInfo constructorInfo)
		{
			Type declaringType = constructorInfo.DeclaringType;

			if (declaringType == null)
			{
				throw new ArgumentException($"Declaring type of constructor {constructorInfo.Name} is null, wtf");
			}

			Ctor ctor = new Ctor(declaringType.FullName)
			{
				AccessModifyer = CtorCore.GetAccessModifyer(constructorInfo)
			};

			foreach (var ctorBind in constructorInfo.GetCustomAttributes<CtorBindAttribute>())
			{
				string ctorArgTypeName = Bag.Pocket[ctorBind.Name]; // TODO generic info to bag
				CtorArgument arg = new CtorArgument(ctorBind.Name, ctorArgTypeName, ctorBind.Settings);

				ctor.ArgumentCollection.Add(arg);
			}

			return ctor;
		}

		private static AccessModifyer GetAccessModifyer(ConstructorInfo constructorInfo)
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
	}
}