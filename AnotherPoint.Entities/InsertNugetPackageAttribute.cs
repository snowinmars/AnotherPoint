using System;
using System.Text;

namespace AnotherPoint.Entities
{
	public class InsertNugetPackageAttribute : Attribute
	{
		public InsertNugetPackageAttribute(string name, int majorVersion, int minorVersion, int revisionVersion, int buildVersion, string culture, string processorArchitecture)
		{
			this.Name = name;
			this.Version = new Version(majorVersion, minorVersion, revisionVersion, buildVersion);
			this.Culture = culture;
			this.ProcessorArchitecture = processorArchitecture;
		}

		public string Name { get; }
		public Version Version { get; }
		public string Culture { get; }
		public string ProcessorArchitecture { get; }

		public string ReferenceInclude
		{
			get { return $"{this.Name}, Version={this.Version}, Culture={this.Culture}, processorArchitecture={this.ProcessorArchitecture}"; }
		}

		public string HintPath
		{
			get
			{
				StringBuilder sb = new StringBuilder();

				sb.Append(this.Version.Major);
				sb.Append(".");
				sb.Append(this.Version.Minor);
				sb.Append(".");
				sb.Append(this.Version.Build);

				return $"..\\packages\\{this.Name}.{sb}\\lib\\net451\\{this.Name}.dll";
			}
		}
	}
}