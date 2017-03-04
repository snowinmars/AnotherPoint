using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AnotherPoint.Common;
using AnotherPoint.Entities;

namespace AnotherPoint.Core
{
	public static class FieldCore
	{
		public static Field Map(FieldInfo fieldInfo)
		{
			string fieldName = fieldInfo.Name;
			string fieldType = Helpers.GetCorrectCollectionTypeNaming(fieldInfo.FieldType.Name);

			Field field = new Field(fieldName, fieldType)
			{
				AccessModifyer = GetAccessModifyer(fieldInfo)
			};

			SetupGeneric(fieldInfo, field);

			// saving field name and type for further appeals from ctor

			Bag.Pocket[fieldName] = fieldType;

			return field;
		}

		private static void SetupGeneric(FieldInfo fieldInfo, Field field)
		{
			field.Type.IsGeneric = fieldInfo.FieldType.IsGenericType;
			foreach (var genericTypeArgument in fieldInfo.FieldType.GenericTypeArguments)
			{
				field.Type.GenericTypes.Add(genericTypeArgument.FullName);
			}
		}

		private static AccessModifyer GetAccessModifyer(FieldInfo fieldInfo)
		{
			AccessModifyer accessModifyer = AccessModifyer.None;

			if (fieldInfo.IsPublic)
			{
				accessModifyer |= AccessModifyer.Public;
			}

			if (fieldInfo.IsProtected())
			{
				accessModifyer |= AccessModifyer.Protected;
			}

			if (fieldInfo.IsPrivate)
			{
				accessModifyer |= AccessModifyer.Private;
			}

			if (fieldInfo.IsInternal())
			{
				accessModifyer |= AccessModifyer.Internal;
			}

			if (fieldInfo.IsProtectedInternal())
			{
				accessModifyer |= AccessModifyer.Internal | AccessModifyer.Protected;
			}

			return accessModifyer;
		}
	}
}
