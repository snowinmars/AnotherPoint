using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnotherPoint.Common
{
    public enum TemplateType
    {
        None = 0,
        Class = 1,
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
                default:
                    throw new ArgumentOutOfRangeException(nameof(t), t, null);
            }
        }
    }
}
