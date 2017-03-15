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
					new SolutionCore());

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
			var endpoint = RenderEngine.EndpointCore.ConstructEndpointFor(userClass);
			RenderEngine.SolutionCore.ConstructSolution(new[] { endpoint }, outputPath);

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
	public class User : IComparable
	{
		[Bind(BindSettings.None, "name")]
		[Bind(BindSettings.CallThis, "name")]
		[Bind(BindSettings.CallThis, "0")]
		public User(string name)
		{
		}

		[Bind(BindSettings.Exact, "name")]
		[Bind(BindSettings.Exact, "age")]
		[Bind(BindSettings.New, "children")]
		public User(string name, int age)
		{
		}

		[Range(7, 12)]
		public int Age { get; set; }

		public IEnumerable<User> Children { get; private set; }

		public Guid Id { get; set; }

		[RegularExpression(".*")]
		public string Name { get; private set; }

		[MethodImpl.ShutMeUp]
		public int CompareTo(object obj)
		{
			throw new NotImplementedException();
		}
	}
}