using System.Reflection;

namespace AnotherPoint.Common
{
	public static class AssemblyForge
	{
		public static void ForgeAll()
		{
			foreach (var assemblyName in AssemblyNames)
			{
				var assembly = Assembly.Load(assemblyName);
			}
		}

		private static readonly string[] AssemblyNames =
		{
			"AnotherPoint.Common",
			"AnotherPoint.Core",
			"AnotherPoint.Entities",
			"AnotherPoint.Templates",
		};
	}
}