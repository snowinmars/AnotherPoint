using AnotherPoint.Common;
using AnotherPoint.Entities;
using AnotherPoint.Extensions;
using System.Reflection;
using AnotherPoint.Interfaces;

namespace AnotherPoint.Core
{
	public class FieldCore : IFieldCore
	{
		public string RenderAccessModifyer(Field field)
		{
			return field.AccessModifyer.AsString();
		}

		public string RenderName(Field field)
		{
			return field.Name;
		}

		public string RenderTypeName(Field field)
		{
			return field.Type.Name;
		}

		public Field Map(FieldInfo fieldInfo)
		{
			string fieldName = fieldInfo.Name;
			string fieldType = Helpers.GetCorrectCollectionTypeNaming(fieldInfo.FieldType.Name);

			Field field = new Field(fieldName, fieldType)
			{
				AccessModifyer = GetAccessModifyer(fieldInfo)
			};

			SetupGeneric(fieldInfo, field);

			// saving field name and type for further appeals from ctor
			Bag.Pocket[fieldName.ToUpperInvariant()] = field.Type;

			return field;
		}

		private AccessModifyer GetAccessModifyer(FieldInfo fieldInfo)
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

		private void SetupGeneric(FieldInfo fieldInfo, Field field)
		{
			field.Type.IsGeneric = fieldInfo.FieldType.IsGenericType;

			foreach (var genericTypeArgument in fieldInfo.FieldType.GenericTypeArguments)
			{
				field.Type.GenericTypes.Add(genericTypeArgument.FullName);
			}
		}

		public void Dispose()
		{
		}
	}
}