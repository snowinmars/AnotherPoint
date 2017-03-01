using System.Reflection;

namespace AnotherPoint.Common
{
	public static class Constant
	{
		public const string Abstract = " abstract ";
		public const string BackgroundEntityMark = "__";

		public const string Internal = " internal ";
		public const string Private = " private ";
		public const string Protected = " protected ";
		public const string ProtectedInternal = " protected internal ";
		public const string Public = " public ";
		public const string Sealed = " sealed ";

		public const string Todo = " TODO ";

		public const BindingFlags AllInstance = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
		public const BindingFlags AllStatic = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
	}
}