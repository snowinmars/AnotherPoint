using System;
using System.Reflection;

namespace AnotherPoint.Common
{
	public static class AssemblyForge
	{
		// ok, that's another crunch here
		// What the problem: comment assemblyPathes block - and AnotherPoint will work only with debugger in debug mode
		// If you try to run sln without debugger, you will have Exception in ValidationCore: Type of DataAttribute will not be found
		// That's because debugger and non-debugger libs are absolutely different, so we have to forge the dll with DataAttributes manually, and we have definitely no choice but forge it by full name
		// TODO add brain to get full name from user's machine

		// And forging assemblies by name is here due to Razor engine need all referenced assemblies forged to memory

		private static readonly string[] AssemblyNames;

		private static readonly string[] AssemblyPathes;

		static AssemblyForge()
		{
			AssemblyForge.AssemblyNames = new[] {
				"AnotherPoint.Common",
				"AnotherPoint.Core",
				"AnotherPoint.Engine",
				"AnotherPoint.Entities",
				"AnotherPoint.Extensions",
				"AnotherPoint.Interfaces",
				"AnotherPoint.Templates",
			};

			AssemblyForge.AssemblyPathes = new[] {
				@"C:\Windows\Microsoft.NET\assembly\GAC_MSIL\System.ComponentModel.DataAnnotations\v4.0_4.0.0.0__31bf3856ad364e35\System.ComponentModel.DataAnnotations.dll",
			};
		}

		public static void ForgeAll()
		{
			Log.Info("Forging assemblies...");

			foreach (var assemblyName in AssemblyForge.AssemblyNames)
			{
				Assembly.Load(assemblyName);
			}

			foreach (var assemblyPath in AssemblyForge.AssemblyPathes)
			{
				Assembly.LoadFile(assemblyPath);
			}

			Log.iDone();
		}
	}
}