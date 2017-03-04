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
		private static readonly DynamicViewBag dynamicViewBag;
		private static readonly string RootFolder = Directory.GetCurrentDirectory();
		private static IDictionary<TemplateType, string> nameFileBinding;
		private static IRazorEngineService razorService;

		static TemplateRepository()
		{
			dynamicViewBag = new DynamicViewBag();

			InitializeNameFileBinding();

			TemplateRepository.SelfValidate();
			TemplateServiceConfiguration config = GetDefaultConfig();

			TemplateRepository.razorService = RazorEngineService.Create(config);
			InitRazorEngineTemplates();
		}

		public static string Compile(TemplateType template, object model)
		{
			var nameOnlyTemplateKey = new NameOnlyTemplateKey(template.AsString(),
															ResolveType.Layout,
															context: null);

			var v = TemplateRepository.razorService.RunCompile(nameOnlyTemplateKey,
																modelType: null,
																model: model,
																viewBag: dynamicViewBag);

			v = v.Replace("&gt;", ">").Replace("&lt;", "<");

			return v;
		}

		private static TemplateServiceConfiguration GetDefaultConfig()
		{
			var config = new TemplateServiceConfiguration
			{
				Language = Language.CSharp,
#if DEBUG
				Debug = true,
#endif
			};

			return config;
		}

		private static void InitializeNameFileBinding()
		{
			TemplateRepository.nameFileBinding = new Dictionary<TemplateType, string>();

			foreach (var enumName in Enum.GetNames(typeof(TemplateType)))
			{
				TemplateType templateType;
				if (!Enum.TryParse(enumName, out templateType))
				{
					throw new InvalidOperationException($"Can't parse enum {nameof(TemplateType)}: string enum value is wrong: it's {enumName}");
				}

				nameFileBinding.Add(templateType, $"{Path.Combine(TemplateRepository.RootFolder, "dat", $"{enumName}.dat")}");
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

				TemplateRepository.razorService.AddTemplate(templateKey,
															templateSource);
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