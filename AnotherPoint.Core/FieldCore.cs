using AnotherPoint.Common;
using AnotherPoint.Entities;
using AnotherPoint.Extensions;
using AnotherPoint.Interfaces;
using System;
using System.Reflection;

namespace AnotherPoint.Core
{
	public class FieldCore : IFieldCore
	{
		public void Dispose()
		{
		}

		public Field Map(FieldInfo fieldInfo)
		{
			string fieldName = fieldInfo.Name;
			string fieldType = Helpers.GetCorrectCollectionTypeNaming(fieldInfo.FieldType.Name);

			Field field = new Field(fieldName, fieldType)
			{
				AccessModifyer = this.GetAccessModifyer(fieldInfo)
			};

			this.SetupGeneric(fieldInfo.FieldType, field.Type);

			// saving field name and type for further appeals from ctor
			Bag.TypePocket[fieldName.ToUpperInvariant()] = field.Type;

			return field;
		}

		public string RenderAccessModifyer(Field field)
		{
			return field.AccessModifyer.AsString();
		}

		public string RenderName(Field field)
		{
			return field.Name.FirstLetterToUpper();
		}

		public string RenderTypeName(Field field)
		{
			return field.Type.Name;
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

		private void SetupGeneric(Type systemFieldType, MyType myType)
		{
			myType.IsGeneric = systemFieldType.IsGenericType;

			foreach (var genericTypeArgument in systemFieldType.GenericTypeArguments)
			{
				myType.GenericTypes.Add(genericTypeArgument.FullName);
			}
		}
	}
}