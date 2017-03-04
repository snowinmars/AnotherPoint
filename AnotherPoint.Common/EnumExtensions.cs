using System;
using System.Text;

namespace AnotherPoint.Common
{
	public static class EnumExtensions
	{
		public static string AsString(this TemplateType t)
		{
			switch (t)
			{
				case TemplateType.Class:
					return "class";

				case TemplateType.Field:
					return "field";

				case TemplateType.Property:
					return "property";

				case TemplateType.Ctor:
					return "ctor";

				default:
					throw new ArgumentOutOfRangeException(nameof(t), t, null);
			}
		}

		public static string AsString(this AccessModifyer t)
		{
			StringBuilder sb = new StringBuilder();

			switch (t)
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

				default:
					throw new ArgumentOutOfRangeException(nameof(t), t, null);
			}

			return sb.ToString();
		}
	}
}