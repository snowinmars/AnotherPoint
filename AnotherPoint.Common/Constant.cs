using System.Collections.Generic;
using System.Reflection;

namespace AnotherPoint.Common
{
	public static class Constant
	{
		#region constants for rendering

		// I wanna be ensure that no words will stick to each other, so I surround this type of constants with spaces

		public const string Abstract = " abstract ";
		public const string Internal = " internal ";
		public const string Private = " private ";
		public const string Protected = " protected ";
		public const string ProtectedInternal = " protected internal ";
		public const string Public = " public ";
		public const string Sealed = " sealed ";
		public const string Todo = " TODO ";
		public const string Virtual = " virtual ";
		public const string Static = " static ";

		#endregion constants for rendering

		public const BindingFlags AllInstance = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
		public const BindingFlags AllStatic = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;

		#region exact matching constants

		// I use this constants for exact matching in a code, so don't space it

		public const string AnotherPoint = "AnotherPoint";
		public const string BackgroundEntityMark = "__";
		public const string Generic = "Generic";
		public const string Get = "get";
		public const string Set = "set";
		public const string Void = "Void";

		#endregion exact matching constants

		public const string DefaultDestination = "DefaultDestination";

		public const string MethodBody_ShutUp = " throw new System.NotImplementedException(); ";

		public static readonly IDictionary<string, string> FullTypeNameHumanReadableBinding = new Dictionary<string, string>
		{
			{ "System.Collections.Generic.IEnumerable`1", "System.Collections.Generic.IEnumerable" }
		};

	}
}