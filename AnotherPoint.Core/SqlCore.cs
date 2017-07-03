using AnotherPoint.Entities;
using AnotherPoint.Extensions;
using AnotherPoint.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AnotherPoint.Core
{
	public class SqlCore : ISqlCore
	{
		private static readonly IDictionary<string, string> SqlTypeMapping;

		static SqlCore()
		{
			SqlCore.SqlTypeMapping = new Dictionary<string, string>
			{
				{"Boolean", "bit"},
				{"Byte", "tinyint"},
				{"Byte[]", "binary"},
				{"Char[]", "char (1000)"},
				{"DateTime", "datetime2"},
				{"DateTimeOffset", "datetimeoffset"},
				{"Decimal", "decimal"},
				{"Double", "float"},
				{"Guid", "uniqueidentifier"},
				{"Int16", "smallint"},
				{"Int32", "int"},
				{"Int64", "bigint"},
				{"Object", "sql_variant"},
				{"Single", "real"},
				{"String", "char (1000)"},
				{"TimeSpan", "time"},
				{"Xml", "xml"},
			};
		}

		public void ConstructSqlScripts(IEnumerable<Endpoint> endpoints, string fullPathToDir)
		{
			foreach (var endpoint in endpoints)
			{
				this.ConstructSqlScript(endpoint, fullPathToDir);
			}
		}

		private void ConstructSqlScript(Endpoint endpoint, string fullPathToDir)
		{
			Class entity = endpoint.EntityClass;

			string createTableCommand = this.GetCreateTableCommand(entity);

			using (var stream = File.Create(Path.Combine(fullPathToDir, $"Create{entity.Name.FirstLetterToUpper()}Table.sql")))
			{
				using (StreamWriter writer = new StreamWriter(stream))
				{
					writer.WriteLine(createTableCommand);
				}
			}
		}

		private string GetCreateTableCommand(Class entity)
		{
			StringBuilder sb = new StringBuilder();

			sb.AppendLine($"CREATE TABLE {entity.Name}s (");

			foreach (var property in entity.Properties)
			{
				if (property.Type.IsCollection.IsTrue())
				{
					string s = $"CREATE TABLE {entity.Name}{property.Type.Name}Binding";
					//todo
				}
				else
				{
					sb.AppendLine($"{property.Name} {this.MapType(property.Type.Name)} , ");
				}
			}

			sb.AppendLine(")");

			return sb.ToString();
		}

		private string MapType(string input)
		{

			if (SqlCore.SqlTypeMapping.TryGetValue(input, out string output))
			{
				return output;
			}

			throw new InvalidOperationException($"There's no type {input} in .NET");
		}
	}
}