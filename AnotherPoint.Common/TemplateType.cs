using System;

namespace AnotherPoint.Common
{
	public enum TemplateType
	{
		Class = 1,
		Field = 2,
		Property = 3,
		Ctor = 4,
	}

	public static class TemplateTypeExtension
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
	}
}