using AnotherPoint.Entities;
using System.Reflection;
using AnotherPoint.Common;

namespace AnotherPoint.Core
{
	public static class PropertyCore
	{
		public static Property Map(PropertyInfo propertyInfo)
		{
			string propertyName = propertyInfo.Name;
			string propertyType = Helpers.GetCorrectCollectionTypeNaming(propertyInfo.PropertyType.FullName);

			Property property = new Property(propertyName, propertyType)
			{
				AccessModifyer = GetGetMethodAccessModifyer(propertyInfo),
				SetMethod =
				{
					AccessModifyer = SetGetMethodAccessModifyer(propertyInfo)
				},
			};

			SetupGeneric(propertyInfo, property);

			// saving field name and type for further appeals from ctor

			Bag.Pocket[propertyName] = propertyType;

			return property;
		}

		private static void SetupGeneric(PropertyInfo propertyInfo, Property property)
		{
			property.Type.IsGeneric = propertyInfo.PropertyType.IsGenericType;
			foreach (var genericTypeArgument in propertyInfo.PropertyType.GenericTypeArguments)
			{
				property.Type.GenericTypes.Add(genericTypeArgument.FullName);
			}
		}

		private static AccessModifyer SetGetMethodAccessModifyer(PropertyInfo propertyInfo)
		{
			AccessModifyer setMethodAccessModifyer = AccessModifyer.None;

			if (propertyInfo.SetMethod.IsPublic)
			{
				setMethodAccessModifyer |= AccessModifyer.Public;
			}

			if (propertyInfo.SetMethod.IsProtected())
			{
				setMethodAccessModifyer |= AccessModifyer.Protected;
			}

			if (propertyInfo.SetMethod.IsPrivate)
			{
				setMethodAccessModifyer |= AccessModifyer.Private;
			}

			if (propertyInfo.SetMethod.IsInternal())
			{
				setMethodAccessModifyer |= AccessModifyer.Internal;
			}

			if (propertyInfo.SetMethod.IsProtectedInternal())
			{
				setMethodAccessModifyer |= AccessModifyer.Internal | AccessModifyer.Protected;
			}

			if (propertyInfo.SetMethod.IsAbstract)
			{
				setMethodAccessModifyer |= AccessModifyer.Abstract;
			}

			return setMethodAccessModifyer;
		}

		private static AccessModifyer GetGetMethodAccessModifyer(PropertyInfo propertyInfo)
		{
			AccessModifyer getMethodAccessModifyer = AccessModifyer.None;

			if (propertyInfo.GetMethod.IsPublic)
			{
				getMethodAccessModifyer |= AccessModifyer.Public;
			}

			if (propertyInfo.GetMethod.IsProtected())
			{
				getMethodAccessModifyer |= AccessModifyer.Protected;
			}

			if (propertyInfo.GetMethod.IsPrivate)
			{
				getMethodAccessModifyer |= AccessModifyer.Private;
			}

			if (propertyInfo.GetMethod.IsInternal())
			{
				getMethodAccessModifyer |= AccessModifyer.Internal;
			}

			if (propertyInfo.GetMethod.IsProtectedInternal())
			{
				getMethodAccessModifyer |= AccessModifyer.Internal | AccessModifyer.Protected;
			}

			if (propertyInfo.GetMethod.IsAbstract)
			{
				getMethodAccessModifyer |= AccessModifyer.Abstract;
			}

			return getMethodAccessModifyer;
		}
	}
}