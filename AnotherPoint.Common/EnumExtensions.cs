using System;
using System.Text;

namespace AnotherPoint.Common
{
	public static class EnumExtensions
	{
		public static string AsString(this TemplateType templateType)
		{
			switch (templateType)
			{
				case TemplateType.Class:
					return "class";

				case TemplateType.Field:
					return "field";

				case TemplateType.Property:
					return "property";

				case TemplateType.Ctor:
					return "ctor";

				case TemplateType.Method:
					return "method";

				case TemplateType.Constant:
					return "constant";

				case TemplateType.Interface:
					return "interface";

				case TemplateType.InterfaceMethod:
					return "interfaceMethod";

				case TemplateType.None:
					return "";

				default:
					throw new ArgumentOutOfRangeException(nameof(templateType), templateType, null);
			}
		}

		public static string AsString(this AccessModifyer accessModifyer)
		{
			if (accessModifyer == AccessModifyer.None)
			{
				return "";
			}

			StringBuilder sb = new StringBuilder();

			if (accessModifyer.HasFlag(AccessModifyer.Public))
			{
				sb.Append(Constant.Public);
			}
			if (accessModifyer.HasFlag(AccessModifyer.Internal))
			{
				sb.Append(Constant.Internal);
			}
			if (accessModifyer.HasFlag(AccessModifyer.Protected))
			{
				sb.Append(Constant.Protected);
			}
			if (accessModifyer.HasFlag(AccessModifyer.Private))
			{
				sb.Append(Constant.Private);
			}
			if (accessModifyer.HasFlag(AccessModifyer.Abstract))
			{
				sb.Append(Constant.Abstract);
			}
			if (accessModifyer.HasFlag(AccessModifyer.Sealed))
			{
				sb.Append(Constant.Sealed);
			}
			if (accessModifyer.HasFlag(AccessModifyer.Virtual))
			{
				sb.Append(Constant.Virtual);
			}
			if (accessModifyer.HasFlag(AccessModifyer.Static))
			{
				sb.Replace(Constant.Abstract, "").Replace(Constant.Sealed, ""); // abstract + sealed == static for clr
				sb.Append(Constant.Static);
			}

			return sb.ToString();
		}
	}
}