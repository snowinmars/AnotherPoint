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

				case TemplateType.None:
					return "";
				default:
					throw new ArgumentOutOfRangeException(nameof(templateType), templateType, null);
			}
		}

		public static string AsString(this AccessModifyer accessModifyer)
		{
			StringBuilder sb = new StringBuilder();

			switch (accessModifyer)
			{
				case AccessModifyer.Public:
					sb.Append(" ");
					sb.Append(Constant.Public);
					sb.Append(" ");
					break;

				case AccessModifyer.Internal:
					sb.Append(" ");
					sb.Append(Constant.Internal);
					sb.Append(" ");
					break;

				case AccessModifyer.Protected:
					sb.Append(" ");
					sb.Append(Constant.Protected);
					sb.Append(" ");
					break;

				case AccessModifyer.Private:
					sb.Append(" ");
					sb.Append(Constant.Private);
					sb.Append(" ");
					break;

				case AccessModifyer.Abstract:
					sb.Append(" ");
					sb.Append(Constant.Abstract);
					sb.Append(" ");
					break;

				case AccessModifyer.Sealed:
					sb.Append(" ");
					sb.Append(Constant.Sealed);
					sb.Append(" ");
					break;

				case AccessModifyer.None:
				default:
					throw new ArgumentOutOfRangeException(nameof(accessModifyer), accessModifyer, null);
			}

			return sb.ToString();
		}
	}
}