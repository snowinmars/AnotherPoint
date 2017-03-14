using AnotherPoint.Common;
using AnotherPoint.Engine;
using AnotherPoint.Entities;
using AnotherPoint.Extensions;
using AnotherPoint.Interfaces;
using AnotherPoint.Templates;
using Microsoft.Build.Construction;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using Microsoft.Build.Evaluation;

namespace AnotherPoint.Core
{
	public class SolutionCore : ISolutionCore
	{
		private readonly IDictionary<string, IList<string>> internalReferences = new Dictionary<string, IList<string>>();
		private readonly IDictionary<string, IList<string>> externalReferences = new Dictionary<string, IList<string>>();
		private string root;

		public void ConstructSolution(IEnumerable<Endpoint> endpoints, string fullPathToDir)
		{
			this.root = fullPathToDir;

			foreach (var endpoint in endpoints)
			{
				this.ScaffoldToDirectory(endpoint, fullPathToDir);
			}

			foreach (var endpoint in endpoints)
			{
				foreach (var @class in endpoint.Classes)
				{
					if (this.internalReferences.ContainsKey(@class.Namespace))
					{
						continue;
					}

					this.internalReferences.Add(@class.Namespace, new List<string>());
					this.externalReferences.Add(@class.Namespace, new List<string>());

					foreach (var @using in @class.Usings)
					{
						if (@using.StartsWith(endpoint.AppName) &&
						    !this.internalReferences[@class.Namespace].Contains(@using))
						{
							this.internalReferences[@class.Namespace].Add(@using);
						}
					}

					foreach (var reference in @class.References)
					{
						this.externalReferences[@class.Namespace].Add(reference);
					}

					if (@class.Validation != null)
					{
						foreach (var reference in @class.Validation.References)
						{
							this.externalReferences[@class.Namespace].Add(reference);
						}
					}
				}

				foreach (var @interface in endpoint.Interfaces)
				{
					if (this.internalReferences.ContainsKey(@interface.Namespace))
					{
						continue;
					}


					this.internalReferences.Add(@interface.Namespace, new List<string>());
					foreach (var @using in @interface.Usings)
					{
						if (@using.StartsWith(endpoint.AppName) &&
							!this.internalReferences[@interface.Namespace].Contains(@using))
						{
							this.internalReferences[@interface.Namespace].Add(@using);
						}
					}

					foreach (var reference in @interface.References)
					{
						this.internalReferences[@interface.Namespace].Add(reference);
					}
				}
			}

			this.WriteSolution(fullPathToDir);
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
			File.WriteAllText(Path.Combine(daoDir.FullName, $"{endpoint.DAOClass.Name}.cs"), renderedDao);

			File.WriteAllText(Path.Combine(this.root, endpoint.BLLClass.Validation.Namespace, $"{Constant.Validation}.cs"), TemplateRepository.Compile(TemplateType.Class, endpoint.BLLClass.Validation));
			File.WriteAllText(Path.Combine(this.root, endpoint.DAOClass.Validation.Namespace, $"{Constant.Validation}.cs"), TemplateRepository.Compile(TemplateType.Class, endpoint.DAOClass.Validation));
		}

		private void WriteSolution(string fullPathToDir)
		{
			IList<DirectoryInfo> res = new List<DirectoryInfo>();

			foreach (var internalReference in this.internalReferences.OrderBy(ir => ir.Value.Count))
			{
				var d = new DirectoryInfo(Path.Combine(fullPathToDir, internalReference.Key));

				res.Add(d);
			}

			foreach (var directoryInfo in res)
			{
				this.WriteProject(directoryInfo);
			}
		}

		private void WriteProject(DirectoryInfo directoryInfo)
		{
			var root = ProjectRootElement.Create();
			this.SetUpDefaultPropertyGroup( root);
			this.SetUpDebugPropertyGroup(directoryInfo, root);
			this.SetUpReleasePropertyGroup( root);

			// references
			ProjectItemGroupElement r = root.AddItemGroup();
			//var proh = r.AddItem("ProjectReference", this.internalReferences[directoryInfo.Name].Select(i => $"..\\{i}\\{i}.csproj").ToArray());

			foreach (var a in this.internalReferences[directoryInfo.Name])
			{
				IList<KeyValuePair<string, string>> asd = new List<KeyValuePair<string, string>>();

				string destinationCsprojPath = $"{a}\\{a}.csproj";

				string destinationCsprojGuid;

				using (FileStream asdfg = File.OpenRead(Path.Combine(this.root, destinationCsprojPath)))
				{
					using (StreamReader reader = new StreamReader(asdfg))
					{
						string line;

						while ((line = reader.ReadLine()) != null)
						{
							if (line.Trim().StartsWith("<ProjectGuid>"))
							{
								destinationCsprojGuid = Regex.Match(line, ">(.*)<").Groups[1].Value;

								asd.Add(new KeyValuePair<string, string>("Project", destinationCsprojGuid));
								asd.Add(new KeyValuePair<string, string>("Name", a));
							}
						}

					}
				}

				ProjectItemElement proh = r.AddItem("ProjectReference", $"..\\{destinationCsprojPath}", asd);

			}

			if (this.externalReferences.ContainsKey(directoryInfo.Name))
			{
				root.AddItems("Reference", this.externalReferences[directoryInfo.Name].ToArray());
			}


			// items to compile
			root.AddItems("Compile", directoryInfo.GetFiles("*.cs").Select(f => f.Name).ToArray());

			root.AddImport(@"$(MSBuildToolsPath)\Microsoft.CSharp.targets");

			root.Save(Path.Combine(directoryInfo.FullName, $"{directoryInfo.Name}.csproj"));
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

		private void SetUpDebugPropertyGroup(DirectoryInfo directoryInfo, ProjectRootElement root)
		{
			ProjectPropertyGroupElement group = root.AddPropertyGroup();

			var configurationProperty = group.AddProperty("Configuration", "Debug");
			configurationProperty.Condition = " '$(Configuration)' == '' ";

			var platformProperty = group.AddProperty("Platform", "x64");
			platformProperty.Condition = " '$(Platform)' == '' ";

			group.AddProperty("ProjectGuid", $"{{{Guid.NewGuid()}}}");
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
	}
}