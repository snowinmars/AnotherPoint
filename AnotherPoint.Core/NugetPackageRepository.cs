using AnotherPoint.Entities;

namespace AnotherPoint.Core
{
	internal static class NugetPackageRepository
	{
		public static InsertNugetPackageAttribute Dapper => new InsertNugetPackageAttribute("Dapper", 1, 50, 2, 0, "neutral", "MSIL");
	}
}