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

			foreach (var parameter in ctor.ArgumentCollection
											.Where(arg => arg.BindAttribute != BindSettings.CallThis && arg.BindAttribute != BindSettings.New))
			{
				args.Append(parameter.Type.FullName);

				if (parameter.Type.IsGeneric.HasValue && parameter.Type.IsGeneric.Value)
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
					case BindSettings.Exact:
						body.Append(" this. ");
						body.Append(bind.Name.FirstLetterToUpper());
						body.Append(" = ");
						body.Append(bind.Name.FirstLetterToLower());
						body.Append(";");
						break;

					case BindSettings.New:
						body.Append(" this. ");
						body.Append(bind.Name.FirstLetterToUpper());
						body.Append(" = ");
						body.Append(" new ");

						MyType type = Bag.Pocket[bind.Name];

						var fullTypeNameWithoutAssmblyInfo = type.FullName.Split(new[] { '[' }, StringSplitOptions.RemoveEmptyEntries).First();
						var fullImplementTypeName = Helpers.GetImplementTypeNaming(fullTypeNameWithoutAssmblyInfo);
						body.Append(fullImplementTypeName);

						if (type.IsGeneric.HasValue && 
							type.IsGeneric.Value)
						{
							body.Append("<");
							body.Append(string.Join(",", type.GenericTypes));
							body.Append(">");
						}

						if (Helpers.GetImplementTypeNaming(fullTypeNameWithoutAssmblyInfo).Contains(Constant.Generic))
						{
							var v = type.FullName.IndexOf("<", StringComparison.InvariantCultureIgnoreCase);

							if (v >= 0)
							{
								body.Append(type.FullName.Substring(v));
							}
						}
						body.Append("();");
						break;

					case BindSettings.CallThis:
						// do nothing here: I handle it in GetCtorCarriage()
						break;
					case BindSettings.None:
						break;
						throw new ArgumentException($"Enum {nameof(BindSettings)} can't equals to None");
					default:
						throw new ArgumentOutOfRangeException(nameof(bind), bind, $"Enum {nameof(BindSettings)} is out of range");
				}

				body.AppendLine();
			}

			return body.ToString();
		}

		public static string GetCtorCarriage(Ctor ctor)
		{
			StringBuilder body = new StringBuilder(256);

			foreach (var bind in ctor.ArgumentCollection)
			{
				switch (bind.BindAttribute)
				{
					case BindSettings.CallThis:
						body.Append(bind.Name.FirstLetterToLower());
						body.Append(",");
						break;
					case BindSettings.Exact:
						// do nothing here: I handle it in GetBodyAsString()
						break;
					case BindSettings.New:
						// do nothing here: I handle it in GetBodyAsString()
						break;
					case BindSettings.None:
						break;
						throw new ArgumentException($"Enum {nameof(BindSettings)} can't equals to None");
					default:
						throw new ArgumentOutOfRangeException(nameof(bind), bind, $"Enum {nameof(BindSettings)} is out of range");
				}
			}


			if (body.Length > 0)
			{
				body.Remove(body.Length - 1, 1); // removing last comma
				body.Insert(0, " : this(");
				body.Append(")");
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

			foreach (var ctorBind in constructorInfo.GetCustomAttributes<BindAttribute>())
			{
				if (ctorBind.Settings == BindSettings.CallThis)
				{
					Argument arg = new Argument(ctorBind.Name, "System", BindSettings.CallThis);

					ctor.ArgumentCollection.Add(arg);
				}
				else
				{
					MyType argType = Bag.Pocket[ctorBind.Name];
					Argument arg = new Argument(ctorBind.Name, argType.FullName, ctorBind.Settings)
					{
						Type = argType
					};

					ctor.ArgumentCollection.Add(arg);
				}
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