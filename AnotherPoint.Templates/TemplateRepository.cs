using AnotherPoint.Common;
using RazorEngine;
using RazorEngine.Configuration;
using RazorEngine.Templating;
using SandS.Algorithm.Extensions.EnumerableExtensionNamespace;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AnotherPoint.Templates
{
	public static class TemplateRepository
	{
		private static readonly DynamicViewBag DynamicViewBag;
		private static readonly string RootFolder;
		private static IDictionary<TemplateType, string> nameFileBinding;
		private static IRazorEngineService razorService;

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

		public static string Compile(TemplateType template, object model)
		{
			var nameOnlyTemplateKey = new NameOnlyTemplateKey(template.AsString(),
															ResolveType.Layout,
															context: null);

			string str = TemplateRepository.razorService.RunCompile(nameOnlyTemplateKey,
																modelType: null,
																model: model,
																viewBag: TemplateRepository.DynamicViewBag);

			str = str.Replace("&gt;", ">").Replace("&lt;", "<");

			return str;
		}

		private static TemplateServiceConfiguration GetDefaultConfig()
		{
			var config = new TemplateServiceConfiguration
			{
				Language = Language.CSharp,
			};

			return config;
		}

		private static void InitializeNameFileBinding()
		{
			TemplateRepository.nameFileBinding = new Dictionary<TemplateType, string>();

			foreach (var enumName in Enum.GetNames(typeof(TemplateType))
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
			foreach (var pair in TemplateRepository.nameFileBinding)
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