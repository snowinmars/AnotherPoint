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
				sb.Append(" ");
				sb.Append(Constant.Public);
				sb.Append(" ");
			}
			else if (accessModifyer.HasFlag(AccessModifyer.Internal))
			{
				sb.Append(" ");
				sb.Append(Constant.Internal);
				sb.Append(" ");
			}
			else if (accessModifyer.HasFlag(AccessModifyer.Protected))
			{
				sb.Append(" ");
				sb.Append(Constant.Protected);
				sb.Append(" ");
			}
			else if (accessModifyer.HasFlag(AccessModifyer.Private))
			{
				sb.Append(" ");
				sb.Append(Constant.Private);
				sb.Append(" ");
			}
			else if (accessModifyer.HasFlag(AccessModifyer.Abstract))
			{
				sb.Append(" ");
				sb.Append(Constant.Abstract);
				sb.Append(" ");
			}
			else if (accessModifyer.HasFlag(AccessModifyer.Sealed))
			{
				sb.Append(" ");
				sb.Append(Constant.Sealed);
				sb.Append(" ");
			}
			else if (accessModifyer.HasFlag(AccessModifyer.Virtual))
			{
				sb.Append(" ");
				sb.Append(Constant.Virtual);
				sb.Append(" ");
			}
			else
			{
				throw new ArgumentOutOfRangeException(nameof(accessModifyer), accessModifyer, null);
			}

			return sb.ToString();
		}
	}
}