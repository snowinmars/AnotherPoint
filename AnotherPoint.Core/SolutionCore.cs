using AnotherPoint.Common;
using AnotherPoint.Entities;
using AnotherPoint.Extensions;
using AnotherPoint.Interfaces;
using AnotherPoint.Templates;
using Microsoft.Build.Construction;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace AnotherPoint.Core
{
	public class SolutionCore : ISolutionCore
	{
		private readonly IDictionary<string, IList<string>> externalReferences;
		private readonly IDictionary<string, IList<string>> internalReferences;
		private readonly IDictionary<string, IList<InsertNugetPackageAttribute>> packages;
		private readonly IDictionary<string, string> projectIds;
		private string appName;
		private string root;

		public SolutionCore()
		{
			this.externalReferences = new Dictionary<string, IList<string>>();
			this.internalReferences = new Dictionary<string, IList<string>>();
			this.packages = new Dictionary<string, IList<InsertNugetPackageAttribute>>();
			this.projectIds = new Dictionary<string, string>();
		}

		public void ConstructSolution(IEnumerable<Endpoint> endpoints, string fullPathToDir)
		{
			Log.Info($"Constructing solution to {fullPathToDir}...");

			Stopwatch sw = Stopwatch.StartNew();

			this.root = fullPathToDir;

			DirectoryInfo rootDirectory = this.PrepareDirectory(fullPathToDir);

			this.appName = endpoints.First().AppName;

			foreach (var endpoint in endpoints)
			{
				this.ScaffoldEndpoint(endpoint, rootDirectory);
				this.SetupReferences(endpoint);
			}

			this.WriteApplication(fullPathToDir);

			sw.Stop();

			Log.iDone(sw.Elapsed.TotalMilliseconds);
		}

		private void DeclareExistance(string classNamespace)
		{
			string folderName = Helpers.CutNamespaceToInterface(classNamespace);

			if (!this.internalReferences.ContainsKey(folderName))
			{
				this.internalReferences.Add(folderName, new List<string>());
			}

			if (!this.externalReferences.ContainsKey(folderName))
			{
				this.externalReferences.Add(folderName, new List<string>());
			}

			if (!this.packages.ContainsKey(folderName))
			{
				this.packages.Add(folderName, new List<InsertNugetPackageAttribute>());
			}
		}

		private string GetPackagesConfigBody(DirectoryInfo directoryInfo, ProjectRootElement root)
		{
			StringBuilder sb = new StringBuilder();

			sb.AppendLine(@"<?xml version=""1.0"" encoding=""utf-8""?>");
			sb.AppendLine("<packages>");

			if (this.packages.ContainsKey(directoryInfo.Name))
			{
				foreach (var reference in this.packages[directoryInfo.Name])
				{
					// here I have to place info about package into to buckets: the first one is csproj (I use root entity to access it) and the second one is packages.config
					root.AddItem("Reference", reference.ReferenceInclude, new[]
					{
						new KeyValuePair<string, string>("HintPath", reference.HintPath),
						new KeyValuePair<string, string>("Private", "True"),
					});

					sb.AppendLine($"  <package id=\"{reference.Name}\" version=\"{reference.Version}\" targetFramework=\"net452\" />");
				}
			}

			sb.AppendLine("</packages>");

			return sb.ToString();
		}

		private string GetSolutionBody()
		{
			#region consts

			const string slnHeader = @"
Microsoft Visual Studio Solution File, Format Version 12.00
# Visual Studio 14
VisualStudioVersion = 14.0.25420.1
MinimumVisualStudioVersion = 10.0.40219.1";
			const string slnFooterStart = @"
Global
	GlobalSection(SolutionConfigurationPlatforms) = preSolution
		Debug|Any CPU = Debug|Any CPU
		Release|Any CPU = Release|Any CPU
	EndGlobalSection
	GlobalSection(ProjectConfigurationPlatforms) = postSolution";
			const string SlnFooterEnd = @"
EndGlobalSection
	GlobalSection(SolutionProperties) = preSolution
		HideSolutionNode = FALSE
	EndGlobalSection
EndGlobal";

			#endregion consts

			Guid slnId = Guid.NewGuid();

			StringBuilder sb = new StringBuilder();

			sb.AppendLine(slnHeader);

			foreach (var projectId in this.projectIds)
			{
				sb.AppendLine($"Project(\"{{{slnId}}}\") = \"{projectId.Key}\", \"{projectId.Key}\\{projectId.Key}.csproj\", \"{{{projectId.Value}}}\"");
				sb.AppendLine("EndProject");
			}

			sb.AppendLine(slnFooterStart);

			foreach (var projectId in this.projectIds)
			{
				sb.AppendLine($"{projectId.Value}.Debug|Any CPU.ActiveCfg = Debug|Any CPU");
				sb.AppendLine($"{projectId.Value}.Debug|Any CPU.Build.0 = Debug|Any CPU");
				sb.AppendLine($"{projectId.Value}.Release|Any CPU.ActiveCfg = Release|Any CPU");
				sb.AppendLine($"{projectId.Value}.Release|Any CPU.Build.0 = Release|Any CPU");
			}

			sb.AppendLine(SlnFooterEnd);

			return sb.ToString();
		}

		private DirectoryInfo PrepareDirectory(string fullPathToDir)
		{
			if (!Directory.Exists(fullPathToDir))
			{
				Directory.CreateDirectory(fullPathToDir);
			}

			DirectoryInfo rootDirectory = new DirectoryInfo(fullPathToDir);

			rootDirectory.Clear();

			return rootDirectory;
		}

		private void RenderBll(Endpoint endpoint, DirectoryInfo rootDirectory)
		{
			string renderedBll = TemplateRepository.Compile(TemplateType.Class, endpoint.BllClass);
			DirectoryInfo bllDir = rootDirectory.CreateSubdirectory($"{endpoint.AppName}.{Constant.Bll}");

			File.WriteAllText(path: Path.Combine(bllDir.FullName, $"{endpoint.BllClass.Name}.cs"),
								contents: renderedBll);
		}

		private void RenderBllInterfaces(Endpoint endpoint, DirectoryInfo rootDirectory, IDictionary<string, string> renderedBllInterfaces)
		{
			DirectoryInfo bllInterfacesDir = rootDirectory.CreateSubdirectory($"{endpoint.AppName}.{Constant.Bll}.{Constant.Interfaces}");

			foreach (var ibll in renderedBllInterfaces)
			{
				string fileName = Helpers.NameWithoutGeneric(ibll.Key);

				File.WriteAllText(Path.Combine(bllInterfacesDir.FullName, $"{fileName}.cs"), ibll.Value);
			}
		}

		private IDictionary<string, string> RenderBllInterfaces(Endpoint endpoint)
		{
			IDictionary<string, string> renderedBllInterfaces = new Dictionary<string, string>();

			foreach (var endpointBllInterface in endpoint.BllInterfaces)
			{
				string name = endpointBllInterface.Name;
				string @interface = TemplateRepository.Compile(TemplateType.Interface, endpointBllInterface);

				renderedBllInterfaces.Add(name, @interface);
			}

			return renderedBllInterfaces;
		}

		private void RenderCommonClass(Endpoint endpoint, DirectoryInfo rootDirectory)
		{
			string renderedCommon = TemplateRepository.Compile(TemplateType.Class, endpoint.CommonClass);
			DirectoryInfo commonDir = rootDirectory.CreateSubdirectory($"{endpoint.AppName}.{Constant.Common}");

			File.WriteAllText(path: Path.Combine(commonDir.FullName, $"{endpoint.CommonClass.Name}.cs"),
								contents: renderedCommon);
		}

		private void RenderDao(Endpoint endpoint, DirectoryInfo rootDirectory)
		{
			string renderedDao = TemplateRepository.Compile(TemplateType.Class, endpoint.DaoClass);
			DirectoryInfo daoDir = rootDirectory.CreateSubdirectory($"{endpoint.AppName}.{Constant.Dao}");

			File.WriteAllText(path: Path.Combine(daoDir.FullName, $"{endpoint.DaoClass.Name}.cs"),
								contents: renderedDao);
		}

		private IDictionary<string, string> RenderDaoInterfaces(Endpoint endpoint)
		{
			IDictionary<string, string> renderedDaoInterfaces = new Dictionary<string, string>();

			foreach (var endpointDaoInterface in endpoint.DaoInterfaces)
			{
				string name = endpointDaoInterface.Name;
				string @interface = TemplateRepository.Compile(TemplateType.Interface, endpointDaoInterface);

				renderedDaoInterfaces.Add(name, @interface);
			}

			return renderedDaoInterfaces;
		}

		private void RenderEntity(Endpoint endpoint, DirectoryInfo rootDirectory)
		{
			string renderedEntity = TemplateRepository.Compile(TemplateType.Class, endpoint.EntityClass);
			DirectoryInfo entitiesDir = rootDirectory.CreateSubdirectory($"{endpoint.AppName}.{Constant.Entities}");

			File.WriteAllText(path: Path.Combine(entitiesDir.FullName, $"{endpoint.EntityClass.Name}.cs"),
								contents: renderedEntity);
		}

		private void RendernDaoInterfaces(Endpoint endpoint, DirectoryInfo rootDirectory, IDictionary<string, string> renderedDaoInterfaces)
		{
			DirectoryInfo daoInterfacesDir = rootDirectory.CreateSubdirectory($"{endpoint.AppName}.{Constant.Dao}.{Constant.Interfaces}");

			foreach (var idao in renderedDaoInterfaces)
			{
				string fileName = Helpers.NameWithoutGeneric(idao.Key);

				File.WriteAllText(Path.Combine(daoInterfacesDir.FullName, $"{fileName}.cs"), idao.Value);
			}
		}

		private void RenderValidationForBll(Endpoint endpoint, string bllClassValidationNamespace)
		{
			File.WriteAllText(path: Path.Combine(this.root, bllClassValidationNamespace, $"{Constant.Validation}.cs"),
								contents: TemplateRepository.Compile(TemplateType.Class, endpoint.BllClass.Validation));
		}

		private void RenderValidationForDao(Endpoint endpoint, string daoClassValidationNamespace)
		{
			File.WriteAllText(path: Path.Combine(this.root, daoClassValidationNamespace, $"{Constant.Validation}.cs"),
								contents: TemplateRepository.Compile(TemplateType.Class, endpoint.DaoClass.Validation));
		}

		private void SaveProject(DirectoryInfo directoryInfo, ProjectRootElement root)
		{
			root.Save(Path.Combine(directoryInfo.FullName, $"{directoryInfo.Name}.csproj"));
		}

		private void ScaffoldEndpoint(Endpoint endpoint, DirectoryInfo rootDirectory)
		{
			IDictionary<string, string> renderedBllInterfaces = this.RenderBllInterfaces(endpoint);
			IDictionary<string, string> renderedDaoInterfaces = this.RenderDaoInterfaces(endpoint);

			this.RenderCommonClass(endpoint, rootDirectory);
			this.RenderEntity(endpoint, rootDirectory);
			this.RenderBllInterfaces(endpoint, rootDirectory, renderedBllInterfaces);
			this.RendernDaoInterfaces(endpoint, rootDirectory, renderedDaoInterfaces);
			this.RenderBll(endpoint, rootDirectory);
			this.RenderDao(endpoint, rootDirectory);

			// validation classes have to be rendered after its' parent classes
			this.RenderValidationForBll(endpoint, endpoint.BllClass.Validation.Namespace);
			this.RenderValidationForDao(endpoint, endpoint.DaoClass.Validation.Namespace);
		}

		private void SetupCompileTargets(DirectoryInfo directoryInfo, ProjectRootElement root)
		{
			root.AddItems("Compile", directoryInfo.GetFiles("*.cs").Select(f => f.Name).ToArray());
		}

		private void SetUpDebugPropertyGroup(DirectoryInfo directoryInfo, ProjectRootElement root, Guid projectId)
		{
			ProjectPropertyGroupElement group = root.AddPropertyGroup();

			var configurationProperty = group.AddProperty("Configuration", "Debug");
			configurationProperty.Condition = " '$(Configuration)' == '' ";

			var platformProperty = group.AddProperty("Platform", "x64");
			platformProperty.Condition = " '$(Platform)' == '' ";

			group.AddProperty("ProjectGuid", $"{{{projectId}}}");
			group.AddProperty("OutputType", "Library");
			group.AddProperty("RootNamespace", directoryInfo.Name);
			group.AddProperty("TargetFrameworkVersion", "4.6.2");
		}

		private void SetUpDefaultPropertyGroup(ProjectRootElement root)
		{
			ProjectPropertyGroupElement group = root.AddPropertyGroup();

			group.Condition = " '$(Configuration)|$(Platform)' == 'Debug|AnyCPU' ";

			group.AddProperty("DebugSymbols", "true");
			group.AddProperty("DebugType", "full");
			group.AddProperty("Optimize", "false");
			group.AddProperty("OutputPath", @"bin\Debug\");
			group.AddProperty("DefineConstants", "DEBUG;TRACE");
			group.AddProperty("ErrorReport", "prompt");
			group.AddProperty("WarningLevel", "4");
		}

		private void SetupExternalReferences(Class @class)
		{
			foreach (var reference in @class.References)
			{
				this.externalReferences[@class.Namespace].AddOnce(reference);
			}

			if (@class.Validation != null)
			{
				foreach (var reference in @class.Validation.References)
				{
					this.externalReferences[@class.Namespace].AddOnce(reference);
				}
			}
		}

		private void SetupImports(ProjectRootElement root)
		{
			root.AddImport(@"$(MSBuildToolsPath)\Microsoft.CSharp.targets");
		}

		private void SetupInternalPackages(Endpoint endpoint, Interface @interface)
		{
			foreach (var @using in @interface.Usings.Where(u => u.StartsWith(endpoint.AppName)))
			{
				this.internalReferences[@interface.Namespace].AddOnce(@using);
			}

			foreach (var reference in @interface.References)
			{
				this.internalReferences[@interface.Namespace].AddOnce(reference);
			}
		}

		private void SetupInternalReferences(Endpoint endpoint, Class @class)
		{
			foreach (var @using in @class.Usings.Where(u => u.StartsWith(endpoint.AppName)))
			{
				this.internalReferences[@class.Namespace].AddOnce(@using);
			}
		}

		private void SetupPackages(Class @class)
		{
			foreach (var insertNugetPackageAttribute in @class.PackageAttributes)
			{
				this.packages[@class.Namespace].AddOnce(insertNugetPackageAttribute);
			}
		}

		private void SetupPackagesConfig(DirectoryInfo directoryInfo, ProjectRootElement root)
		{
			string body = this.GetPackagesConfigBody(directoryInfo, root);
			this.WritePackagesConfigBody(directoryInfo, body);

			root.AddItem("None", "packages.config");
		}

		private void SetupReferences(Endpoint endpoint)
		{
			if (!this.internalReferences.ContainsKey(endpoint.CommonClass.Namespace))
			{
				this.internalReferences.Add(endpoint.CommonClass.Namespace, new List<string>());
			}

			foreach (var @class in endpoint.Classes)
			{
				if (this.internalReferences.ContainsKey(@class.Namespace))
				{
					continue;
				}

				this.DeclareExistance(@class.Namespace);

				this.SetupInternalReferences(endpoint, @class);
				this.SetupExternalReferences(@class);
				this.SetupPackages(@class);
			}

			foreach (var @interface in endpoint.Interfaces)
			{
				if (this.internalReferences.ContainsKey(@interface.Namespace))
				{
					continue;
				}

				this.DeclareExistance(@interface.Namespace);
				this.SetupInternalPackages(endpoint, @interface);
			}
		}

		private void SetupReferencesToForeignProjects(DirectoryInfo directoryInfo, ProjectRootElement root)
		{
			if (this.externalReferences.ContainsKey(directoryInfo.Name))
			{
				var group = root.AddItemGroup();

				foreach (var item in this.externalReferences[directoryInfo.Name].ToArray())
				{
					if (item != null)
					{
						group.AddItem("Reference", item);
					}
				}
			}
		}

		private void SetupReferenceToMyProjects(DirectoryInfo directoryInfo, ProjectRootElement root)
		{
			ProjectItemGroupElement projectReferenceGroup = root.AddItemGroup();

			foreach (var projectName in this.internalReferences[directoryInfo.Name])
			{
				string destinationCsprojPath = $"{projectName}\\{projectName}.csproj";

				IList<KeyValuePair<string, string>> metadata = new List<KeyValuePair<string, string>>();

				metadata.Add(new KeyValuePair<string, string>("Project", $"{{{this.projectIds[Helpers.CutNamespaceToInterface(projectName)]}}}"));
				metadata.Add(new KeyValuePair<string, string>("Name", projectName));

				projectReferenceGroup.AddItem("ProjectReference", $"..\\{destinationCsprojPath}", metadata);
			}
		}

		private void SetUpReleasePropertyGroup(ProjectRootElement root)
		{
			ProjectPropertyGroupElement group = root.AddPropertyGroup();

			group.Condition = " '$(Configuration)|$(Platform)' == 'Release|AnyCPU' ";

			group.AddProperty("DebugType", "pdbonly");
			group.AddProperty("Optimize", "true");
			group.AddProperty("OutputPath", @"bin\Release\");
			group.AddProperty("DefineConstants", "TRACE");
			group.AddProperty("ErrorReport", "prompt");
			group.AddProperty("WarningLevel", "4");
		}

		private void WriteApplication(string fullPathToDir)
		{
			// that's a bit tricky
			// It's too lazy for me to build correct dependency tree, so I make a guess:
			// if I write projects from those, which have fewer dependencies, I will have correct scaffold order.
			// Yea, that's a crunch, so some day I will have to fix it

			foreach (var directoryInfo in this.internalReferences
											.OrderBy(ir => ir.Value.Count)
											.Select(i => new DirectoryInfo(Path.Combine(fullPathToDir, Helpers.NameWithoutGeneric(i.Key))))
											.ToList())
			{
				this.WriteProject(directoryInfo);
			}

			this.WriteSolution(fullPathToDir);
		}

		private void WritePackagesConfigBody(DirectoryInfo directoryInfo, string packagesConfigBody)
		{
			using (FileStream stream = File.Create(Path.Combine(directoryInfo.FullName, "packages.config")))
			{
				using (TextWriter writer = new StreamWriter(stream))
				{
					writer.WriteLine(packagesConfigBody);
				}
			}
		}

		private void WriteProject(DirectoryInfo directoryInfo)
		{
			var root = ProjectRootElement.Create();

			Guid projectId = Guid.NewGuid();

			if (!this.projectIds.ContainsKey(directoryInfo.Name))
			{
				this.projectIds.Add(directoryInfo.Name, projectId.ToString());
			}

			this.SetUpDefaultPropertyGroup(root);
			this.SetUpDebugPropertyGroup(directoryInfo, root, projectId);
			this.SetUpReleasePropertyGroup(root);
			this.SetupReferenceToMyProjects(directoryInfo, root);
			this.SetupReferencesToForeignProjects(directoryInfo, root);
			this.SetupPackagesConfig(directoryInfo, root);
			this.SetupCompileTargets(directoryInfo, root);
			this.SetupImports(root);
			this.SaveProject(directoryInfo, root);
		}

		private void WriteSolution(string fullPathToDir)
		{
			string body = this.GetSolutionBody();
			this.WriteSolutionBody(fullPathToDir, body);
		}

		private void WriteSolutionBody(string fullPathToDir, string body)
		{
			using (var stream = File.Create(Path.Combine(fullPathToDir, $"{this.appName}.sln")))
			{
				using (StreamWriter writer = new StreamWriter(stream))
				{
					writer.WriteLine(body);
				}
			}
		}
	}
}