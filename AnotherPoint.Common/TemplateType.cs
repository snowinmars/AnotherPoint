using System;

namespace AnotherPoint.Common
{
    public enum TemplateType
    {
        None = 0,
        Class = 1,
        Field = 2,
		Property = 3,
	}

    public static class TemplateTypeExtension
    {
        public static string AsString(this TemplateType t)
        {
            switch (t)
            {
                case TemplateType.None:
                    return "None";

                case TemplateType.Class:
                    return "class";

                case TemplateType.Field:
                    return "field";

				case TemplateType.Property:
		            return "property";

				default:
                    throw new ArgumentOutOfRangeException(nameof(t), t, null);
            }
        }
    }
}