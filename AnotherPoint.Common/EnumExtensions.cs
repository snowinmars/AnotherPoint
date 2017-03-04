using System;

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
			switch (t)
			{
				case AccessModifyer.Public:
					return Constant.Public;

				case AccessModifyer.Internal:
					return Constant.Internal;

				case AccessModifyer.Protected:
					return Constant.Protected;

				case AccessModifyer.ProtectedInternal:
					return Constant.ProtectedInternal;

				case AccessModifyer.Private:
					return Constant.Private;

				default:
					throw new ArgumentOutOfRangeException(nameof(t), t, null);
			}
		}
	}
}