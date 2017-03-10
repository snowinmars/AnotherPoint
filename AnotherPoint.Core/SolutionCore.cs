using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using AnotherPoint.Common;
using AnotherPoint.Engine;
using AnotherPoint.Entities;
using AnotherPoint.Extensions;
using AnotherPoint.Interfaces;
using AnotherPoint.Templates;
using Microsoft.Build.Construction;

namespace AnotherPoint.Core
{
	public class SolutionCore : ISolutionCore
	{
		IDictionary<string, IEnumerable<string>> EntityUsingsBinding = new Dictionary<string, IEnumerable<string>>();


		public void ConstructSolution(IEnumerable<Endpoint> endpoints, string fullPathToDir)
		{
			foreach (var endpoint in endpoints)
			{
				ScaffoldToDirectory(endpoint, fullPathToDir);
			}

			WriteSolution(fullPathToDir);
		}

		private void ScaffoldToDirectory(Endpoint endpoint, string fullPathToDir)
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

				renderedIBlls.Add(name, res);
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
			
			this.EntityUsingsBinding.Add(commonDir.FullName, endpoint.CommonClass.Usings);
			this.EntityUsingsBinding.Add(entitiesDir.FullName, endpoint.EntityClass.Usings);
			this.EntityUsingsBinding.Add(bllInterfacesDir.FullName, endpoint.BLLInterfaces.SelectMany(i => i.Usings));
			this.EntityUsingsBinding.Add(daoInterfacesDir.FullName, endpoint.DAOInterfaces.SelectMany(i => i.Usings));
			this.EntityUsingsBinding.Add(bllDir.FullName, endpoint.BLLClass.Usings);
			this.EntityUsingsBinding.Add(daoDir.FullName, endpoint.DAOClass.Usings);

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
		}

		private void WriteSolution(string fullPathToDir)
		{
			DirectoryInfo rootDir = new DirectoryInfo(fullPathToDir);

			foreach (var directoryInfo in rootDir.GetDirectories())
			{
				WriteProject(directoryInfo);
			}
		}

		private void WriteProject(DirectoryInfo directoryInfo)
		{
			var root = ProjectRootElement.Create();
			SetUpDefaultPropertyGroup(directoryInfo, root);
			SetUpDebugPropertyGroup(directoryInfo, root);
			SetUpReleasePropertyGroup(directoryInfo, root);

			// references
			root.AddItems("Reference", this.EntityUsingsBinding[directoryInfo.FullName].ToArray());

			// items to compile
			root.AddItems("Compile", directoryInfo.GetFiles("*.cs").Select(f => f.Name).ToArray());

			root.AddImport(@"$(MSBuildToolsPath)\Microsoft.CSharp.targets");

			root.Save(Path.Combine(directoryInfo.FullName, $"{directoryInfo.Name}.csproj"));
		}

		private void SetUpReleasePropertyGroup(DirectoryInfo directoryInfo, ProjectRootElement root)
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

		private void SetUpDebugPropertyGroup(DirectoryInfo directoryInfo, ProjectRootElement root)
		{
			ProjectPropertyGroupElement group = root.AddPropertyGroup();

			var configurationProperty = group.AddProperty("Configuration", "Debug");
			configurationProperty.Condition = " '$(Configuration)' == '' ";

			var platformProperty = group.AddProperty("Platform", "x64");
			platformProperty.Condition = " '$(Platform)' == '' ";

			group.AddProperty("ProjectGuid", Guid.NewGuid().ToString());
			group.AddProperty("OutputType", "Library");
			group.AddProperty("RootNamespace", directoryInfo.Name);
			group.AddProperty("TargetFrameworkVersion", "4.6.2");
		}

		private void SetUpDefaultPropertyGroup(DirectoryInfo directoryInfo, ProjectRootElement root)
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
	}
}
