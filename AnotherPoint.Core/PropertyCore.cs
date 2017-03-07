using AnotherPoint.Common;
using AnotherPoint.Entities;
using AnotherPoint.Extensions;
using AnotherPoint.Interfaces;
using System;
using System.Reflection;

namespace AnotherPoint.Core
{
	public class PropertyCore : IPropertyCore
	{
		public string RenderGetMethodAccessModifyer(Property property)
		{
			return property.GetMethod.AccessModifyer.AsString();
		}

		public string RenderName(Property property)
		{
			return property.Name;
		}

		public string RenderSetMethodAccessModifyer(Property property)
		{
			return property.SetMethod.AccessModifyer.AsString();
		}

		public string RenderTypeName(Property property)
		{
			return property.Type.IsGeneric.HasValue && property.Type.IsGeneric.Value ?
						property.Type.FullName + "<" + string.Join(",", property.Type.GenericTypes) + ">" :
						property.Type.FullName;
		}

		public Property Map(PropertyInfo propertyInfo)
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

			SetupGeneric(propertyInfo.PropertyType, property.Type);

			// saving field name and type for further appeals from ctor
			Bag.Pocket[propertyName.ToUpperInvariant()] = property.Type;

			return property;
		}

		private AccessModifyer GetGetMethodAccessModifyer(PropertyInfo propertyInfo)
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

		private AccessModifyer SetGetMethodAccessModifyer(PropertyInfo propertyInfo)
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

		private void SetupGeneric(Type systemPropertyType, MyType propertyMyType)
		{
			propertyMyType.IsGeneric = systemPropertyType.IsGenericType;

			foreach (var genericTypeArgument in systemPropertyType.GenericTypeArguments)
			{
				propertyMyType.GenericTypes.Add(genericTypeArgument.FullName);
			}
		}

		public void Dispose()
		{
		}
	}
}