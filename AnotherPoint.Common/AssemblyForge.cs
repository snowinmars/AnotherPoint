using System.Reflection;

namespace AnotherPoint.Common
{
	public static class AssemblyForge
	{
		private static readonly string[] AssemblyNames =
		{
			"AnotherPoint.Common",
			"AnotherPoint.Core",
			"AnotherPoint.Engine",
			"AnotherPoint.Entities",
			"AnotherPoint.Extensions",
			"AnotherPoint.Interfaces",
			"AnotherPoint.Templates",
		};

		public static void ForgeAll()
		{
			foreach (var assemblyName in AssemblyForge.AssemblyNames)
			{
				Assembly.Load(assemblyName);
			}
		}
	}
}