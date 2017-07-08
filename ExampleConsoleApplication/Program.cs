using FirstApp.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using AnotherPoint.Common;
using AnotherPoint.Core;
using AnotherPoint.Engine;
using AnotherPoint.Entities;
using AnotherPoint.Entities.MethodImpl;
using AnotherPoint.Templates;
using System.ComponentModel.DataAnnotations;

namespace ExampleConsoleApplication
{
	internal class Program : IDisposable
	{
		static Program()
		{
			try
			{
				Log.InitLogger();

				AssemblyForge.ForgeAll();

				RenderEngine.Init(new ClassCore(),
					new CtorCore(),
					new FieldCore(),
					new InterfaceCore(),
					new MethodCore(),
					new PropertyCore(),
					new ValidationCore(),
					new EndpointCore("FirstApp"),
					new SolutionCore(),
					new SqlCore());

				Log.iDone();
			}
			catch (Exception e)
			{
				throw new Exception("Program cctor throws an exception", e);
			}
		}

		private static void Main()
		{
			const string outputPath = @"D:\tmp"; // set this path to the directory you dont need. Directory will be fully eraze

			if (outputPath == null)
			{
				Console.WriteLine("Set output path to the directory you dont need");

				throw new Exception();
			}

			Log.Info("AnotherPoint started");

			Stopwatch sw = Stopwatch.StartNew();

			TemplateRepository.Init();

			Class userClass = RenderEngine.ClassCore.Map(typeof(User));
			Class awardClass = RenderEngine.ClassCore.Map(typeof(Award));

			var userEndpoint = RenderEngine.EndpointCore.ConstructEndpointFor(userClass);
			var awardEndpoint = RenderEngine.EndpointCore.ConstructEndpointFor(awardClass);

			RenderEngine.SolutionCore.ConstructSolution(new[] { userEndpoint, awardEndpoint }, outputPath);
			//RenderEngine.SqlCore.ConstructSqlScripts(new[] { endpoint }, outputPath);

			TemplateRepository.Finit();

			sw.Stop();

			Log.iDone(sw.Elapsed.TotalMilliseconds);
		}

		public void Dispose()
		{
			RenderEngine.Dispose();
		}
	}
}

namespace FirstApp.Entities
{
	[InsertUsing("System")]
	[InsertUsing("FirstApp.Common")]
	[InsertUsing("System.Linq")]
	public class User
	{
		[Bind(BindSettings.None, "name")]
		[Bind(BindSettings.None, "age")]
		[Bind(BindSettings.CallThis, "name")]
		[Bind(BindSettings.CallThis, "\"\"")]
		[Bind(BindSettings.CallThis, "age")]
		public User(string name, int age)
		{
		}

		[Bind(BindSettings.Exact, "name")]
		[Bind(BindSettings.Exact, "surname")]
		[Bind(BindSettings.Exact, "age")]
		public User(string name, string surname, int age)
		{
		}

		[SqlBinding(SqlBindingType.ManyToMany)]
		public IEnumerable<Award> Awards { get; private set; }

		public Guid Id { get; set; }

		[Range(14, 99)]
		public int Age { get; set; }

		public string Name { get; set; }

		public string Surname { get; set; }
	}

	[InsertUsing("System")]
	[InsertUsing("FirstApp.Common")]
	[InsertUsing("System.Linq")]
	public class Award
	{
		[Bind(BindSettings.None, "name")]
		[Bind(BindSettings.CallThis, "name")]
		[Bind(BindSettings.CallThis, "100")]
		public Award(string name)
		{
			
		}

		[Bind(BindSettings.Exact, "name")]
		[Bind(BindSettings.Exact, "rate")]
		public Award(string name, int rate)
		{
			
		}

		public string Name { get; set; }

		[Range(100, 10000)]
		public int Rate { get; set; }
	}
}