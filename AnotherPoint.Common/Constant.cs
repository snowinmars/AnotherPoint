using System.Collections.Generic;
using System.Reflection;

namespace AnotherPoint.Common
{
	public static class Constant
	{
		public const BindingFlags AllInstance = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

		public const BindingFlags AllStatic = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;

		public const string DefaultDestination = "DefaultDestination";

		public const string MethodBodyShutUp = " throw new System.NotImplementedException(); ";

		public static readonly IDictionary<string, string> FullTypeNameHumanReadableBinding;

		public const string TargetFrameworkVersion = "4.6.2";

		static Constant()
		{
			Constant.FullTypeNameHumanReadableBinding = new Dictionary<string, string>
			{
				{ "System.Collections.Generic.IEnumerable`1", "System.Collections.Generic.IEnumerable" }
			};
		}

		public static class Types
		{
			public const string SystemData = "System.Data";
			public const string SystemGuid = "System.Guid";
			public const string SystemInt32 = "System.Int32";
			public const string SystemString = "System.String";
			public const string SystemVoid = "System.Void";
			public const string SystemDataSqlClientSqlConnection = "System.Data.SqlClient.SqlConnection";
		}

		public static class Usings
		{
			public const string Dapper = "Dapper";
			public const string System = "System";
			public const string SystemLinq = "System.Linq";
			public const string SystemTextRegularExpressions = "System.Text.RegularExpressions";
		}

		#region constants for rendering

		// I wanna be ensure that no words will stick to each other, so I surround this type of constants with spaces

		public const string Abstract = " abstract ";
		public const string Internal = " internal ";
		public const string New = " new ";
		public const string Private = " private ";
		public const string Protected = " protected ";
		public const string ProtectedInternal = " protected internal ";
		public const string Public = " public ";
		public const string Sealed = " sealed ";
		public const string Static = " static ";
		public const string This = " this ";
		public const string Todo = " TODO ";
		public const string Virtual = " virtual ";

		#endregion constants for rendering

		#region exact matching constants

		// I use this constants for exact matching in a code, so don't space it

		public const string IEnumerable = "IEnumerable";
		public const string _Constant = "Constant";
		public const string AnotherPoint = "AnotherPoint";
		public const string BackgroundEntityMark = "__";
		public const string Bll = "Bll";
		public const string Common = "Common";
		public const string ConnectionString = "ConnectionString";
		public const string Dal = "DAL";
		public const string Dao = "Dao";
		public const string Entities = "Entities";
		public const string Generic = "Generic";
		public const string Get = "get";
		public const string ICrud = "ICRUD";
		public const string Interfaces = "Interfaces";
		public const string Logic = "Logic";
		public const string Set = "set";
		public const string Void = " void ";
		public const string Return = " return ";

		//public const string Void = "Void";
		public const string Validation = "Validation";

		#endregion exact matching constants
	}
}