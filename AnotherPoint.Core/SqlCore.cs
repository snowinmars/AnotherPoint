using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnotherPoint.Engine;
using AnotherPoint.Entities;
using AnotherPoint.Interfaces;
using System.IO;
using AnotherPoint.Extensions;

namespace AnotherPoint.Core
{
	public class SqlCore : ISqlCore
	{
		public void ConstructSqlScripts(IEnumerable<Endpoint> endpoints, string fullPathToDir)
		{
			foreach (var endpoint in endpoints)
			{
				ConstructSqlScript(endpoint, fullPathToDir);
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

		private static readonly IDictionary<string, string> SqlTypeMapping = new Dictionary<string, string>
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

		private string MapType(string input)
		{
			string output;

			if (SqlCore.SqlTypeMapping.TryGetValue(input, out output))
			{
				return output;
			}

			throw new InvalidOperationException($"There's no type {input} in .NET");
		}
	}
}
