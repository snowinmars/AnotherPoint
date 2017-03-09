using AnotherPoint.Common;
using RazorEngine;
using RazorEngine.Configuration;
using RazorEngine.Templating;
using SandS.Algorithm.Extensions.EnumerableExtensionNamespace;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AnotherPoint.Engine;
using AnotherPoint.Entities;
using AnotherPoint.Extensions;
using Microsoft.Build.Construction;

namespace AnotherPoint.Templates
{
	public static class TemplateRepository
	{
		private static readonly DynamicViewBag DynamicViewBag;
		private static readonly string RootFolder;
		private static IDictionary<TemplateType, string> nameFileBinding;
		private static IRazorEngineService razorService;

		public static void Init()
		{
		}

		public static void Finit()
		{
		}

		static TemplateRepository()
		{
			TemplateRepository.RootFolder = Directory.GetCurrentDirectory();
			TemplateRepository.DynamicViewBag = new DynamicViewBag();

			TemplateRepository.InitializeNameFileBinding();
			TemplateRepository.SelfValidate();

			TemplateServiceConfiguration config = TemplateRepository.GetDefaultConfig();

			TemplateRepository.razorService = RazorEngineService.Create(config);

			TemplateRepository.InitRazorEngineTemplates();
		}

		public static void ScaffoldEndpointToDirectory(Endpoint endpoint, string fullPathToDir)
		{
			if (!Directory.Exists(fullPathToDir))
			{
				Directory.CreateDirectory(fullPathToDir);
			}

			DirectoryInfo d = new DirectoryInfo(fullPathToDir);

			d.Clear();

			IDictionary<string, string> renderedIBlls = new Dictionary<string, string>();

			foreach (var endpointBllInterface in endpoint.BLLInterfaces)
			{
				string res = TemplateRepository.Compile(TemplateType.Interface, endpointBllInterface);
				string name = endpointBllInterface.Name;

				renderedIBlls.Add(name,res);
			}

			IDictionary<string, string> renderedIDaos = new Dictionary<string, string>();

			foreach (var endpointDaoInterface in endpoint.DAOInterfaces)
			{
				string res = TemplateRepository.Compile(TemplateType.Interface, endpointDaoInterface);
				string name = endpointDaoInterface.Name;

				renderedIDaos.Add(name, res);
			}

			string renderedCommon = TemplateRepository.Compile(TemplateType.Class, endpoint.CommonClass);
			string renderedEntity = TemplateRepository.Compile(TemplateType.Class, endpoint.EntityClass);
			string renderedBll = TemplateRepository.Compile(TemplateType.Class, endpoint.BLLClass);
			string renderedDao = TemplateRepository.Compile(TemplateType.Class, endpoint.DAOClass);
			string renderedValidation = TemplateRepository.Compile(TemplateType.Class, RenderEngine.ValidationCore.ConstructValidationClass(endpoint.AppName));

			DirectoryInfo commonDir = d.CreateSubdirectory($"{endpoint.AppName}.{Constant.Common}");
			DirectoryInfo entitiesDir = d.CreateSubdirectory($"{endpoint.AppName}.{Constant.Entities}");
			DirectoryInfo bllInterfacesDir = d.CreateSubdirectory($"{endpoint.AppName}.{Constant.BLL}.{Constant.Interfaces}");
			DirectoryInfo daoInterfacesDir = d.CreateSubdirectory($"{endpoint.AppName}.{Constant.DAO}.{Constant.Interfaces}");
			DirectoryInfo bllDir = d.CreateSubdirectory($"{endpoint.AppName}.{Constant.BLL}");
			DirectoryInfo daoDir = d.CreateSubdirectory($"{endpoint.AppName}.{Constant.DAO}");

			File.WriteAllText(Path.Combine(entitiesDir.FullName, $"{endpoint.EntityClass.Name}.cs"), renderedEntity);
			File.WriteAllText(Path.Combine(commonDir.FullName, $"{endpoint.CommonClass.Name}.cs"), renderedCommon);

			foreach (var ibll in renderedIBlls)
			{
				File.WriteAllText(Path.Combine(bllInterfacesDir.FullName, $"{ibll.Key}.cs"), ibll.Value);
			}

			foreach (var idao in renderedIDaos)
			{
				File.WriteAllText(Path.Combine(daoInterfacesDir.FullName, $"{idao.Key}.cs"), idao.Value);
			}

			File.WriteAllText(Path.Combine(bllDir.FullName, $"{endpoint.BLLClass.Name}.cs"), renderedBll);
			File.WriteAllText(Path.Combine(bllDir.FullName, $"{endpoint.AppName}.cs"), renderedValidation);
			File.WriteAllText(Path.Combine(daoDir.FullName, $"{endpoint.DAOClass.Name}.cs"), renderedDao);

			var root = ProjectRootElement.Create();
			var group = root.AddPropertyGroup();
			group.AddProperty("Configuration", "Debug");
			group.AddProperty("Platform", "x64");

			// references
			root.AddItems("Reference", "System", "System.Core");

			// items to compile
			root.AddItems("Compile", $"{endpoint.CommonClass.Name}.cs");

			var target = root.AddTarget("Build");
			var task = target.AddTask("Csc");
			task.SetParameter("Sources", "@(Compile)");
			task.SetParameter("OutputAssembly", "test.dll");

			root.Save(Path.Combine(commonDir.FullName, $"{endpoint.CommonClass.Name}.csproj"));
		}

		private static void AddItems(this ProjectRootElement elem, string groupName, params string[] items)
		{
			var group = elem.AddItemGroup();
			foreach (var item in items)
			{
				group.AddItem(groupName, item);
			}
		}

		public static string Compile(TemplateType template, object model)
		{
			NameOnlyTemplateKey nameOnlyTemplateKey = new NameOnlyTemplateKey(template.AsString(),
															ResolveType.Layout,
															context: null);

			string str = TemplateRepository.razorService.RunCompile(nameOnlyTemplateKey,
																modelType: null,
																model: model,
																viewBag: TemplateRepository.DynamicViewBag);

			str = str.Replace("&gt;", ">").Replace("&lt;", "<").Replace("&quot;", "\"").Replace("&amp;", "&");

			return str;
		}

		private static TemplateServiceConfiguration GetDefaultConfig()
		{
			TemplateServiceConfiguration config = new TemplateServiceConfiguration
			{
				Language = Language.CSharp,
			};

			return config;
		}

		private static void InitializeNameFileBinding()
		{
			TemplateRepository.nameFileBinding = new Dictionary<TemplateType, string>();

			foreach (string enumName in Enum.GetNames(typeof(TemplateType))
											.Where(e => e != TemplateType.None.ToString()))
			{
				TemplateType templateType;

				if (!Enum.TryParse(enumName, out templateType))
				{
					throw new InvalidOperationException($"Can't parse enum {nameof(TemplateType)}: string enum value is wrong: it's {enumName}");
				}

				// path is {root}/dat/template.dat
				TemplateRepository.nameFileBinding.Add(templateType, $"{Path.Combine(TemplateRepository.RootFolder, "dat", $"{enumName}.dat")}");
			}
		}

		private static void InitRazorEngineTemplates()
		{
			foreach (KeyValuePair<TemplateType, string> pair in TemplateRepository.nameFileBinding)
			{
				ITemplateKey templateKey = new NameOnlyTemplateKey(pair.Key.AsString(),
																	ResolveType.Layout,
																	context: null);

				ITemplateSource templateSource = new LoadedTemplateSource(File.ReadAllText(pair.Value), pair.Value);

				TemplateRepository.razorService.AddTemplate(templateKey, templateSource);
			}
		}

		private static void SelfValidate()
		{
			TemplateRepository.nameFileBinding
				.Values
				.Where(templatePath => !File.Exists(templatePath))
				.ForEach(templatePath =>
				{
					throw new IOException($"Can't find template file {templatePath}");
				});
		}
	}
}