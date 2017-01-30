using AnotherPoint.Common;
using RazorEngine;
using RazorEngine.Configuration;
using RazorEngine.Templating;
using SandS.Algorithm.Extensions.EnumerableExtensionNamespace;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using IRazorEngineService = RazorEngine.Templating.IRazorEngineService;
using NameOnlyTemplateKey = RazorEngine.Templating.NameOnlyTemplateKey;
using RazorEngineService = RazorEngine.Templating.RazorEngineService;
using ResolveType = RazorEngine.Templating.ResolveType;

namespace AnotherPoint.Templates
{
    public static class TemplateRepository
    {
        private static readonly string RootFolder = Directory.GetCurrentDirectory();

        private static readonly IDictionary<TemplateType, string> NameFileBinding;

        private static IRazorEngineService razorService;

        static TemplateRepository()
        {
            TemplateRepository.NameFileBinding = new Dictionary<TemplateType, string>
            {
                {TemplateType.Class, $"{Path.Combine(TemplateRepository.RootFolder, "class.dat")}" }
            };

            TemplateRepository.SelfValidate();

            var config = new TemplateServiceConfiguration
            {
                Language = Language.CSharp,
#if DEBUG
                Debug = true
#endif
            };

            TemplateRepository.razorService = RazorEngineService.Create(config);

            foreach (var pair in TemplateRepository.NameFileBinding)
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
            TemplateRepository.NameFileBinding
                .Values
                .Where(templatePath => !File.Exists(templatePath))
                .ForEach(templatePath =>
                {
                    throw new IOException($"Can't find template file {templatePath}");
                });
        }

        public static string Compile(TemplateType template, object model)
        {
            return TemplateRepository.razorService.RunCompile(new NameOnlyTemplateKey(template.AsString(),
                                                                                ResolveType.Layout,
                                                                                context: null),
                                                        modelType: null,
                                                        model: model,
                                                        viewBag: null);
        }
    }
}