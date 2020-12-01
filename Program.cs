using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;

namespace DbRefFinder {
	public class Program {
		private static readonly Dictionary<string, ReferenceList> sqlReferenceMap = new Dictionary<string, ReferenceList>();

		private static readonly DbAccess dbAccess = new DbAccess();
		private static readonly DirectoryDirector directoryDirector = new DirectoryDirector();
		private static readonly SqlProcInspector sqlProcInspector = new SqlProcInspector();
		private static readonly string reportFileSeparator = new string('=', 50);

		private static string directoryToSearch;

		public static void Main(string[] args) {
			directoryToSearch = ConfigurationManager.AppSettings["DirectoryToSearch"].ToString();

			var tableNames = dbAccess.GetTableNames();
			AddToReferenceMap(tableNames, SqlObjectType.Table);
			var storedProcNames = dbAccess.GetStoredProcNames();
			AddToReferenceMap(storedProcNames, SqlObjectType.StoredProcedure);

			directoryDirector.Scour(directoryToSearch, sqlReferenceMap);
			sqlProcInspector.Inspect(dbAccess, sqlReferenceMap);

			var outputPath = WriteOutputFile(sqlReferenceMap);
			Console.WriteLine($"File written to: {outputPath}");
		}

		private static void AddToReferenceMap(IEnumerable<string> sqlEntityNames, SqlObjectType systemType) {
			foreach (var sqlEntityName in sqlEntityNames) {
				sqlReferenceMap.Add(sqlEntityName, new ReferenceList(sqlEntityName, systemType));
			}
		}

		private static string WriteOutputFile(Dictionary<string, ReferenceList> referenceMap) {
			var pathToFile = Path.Combine(Directory.GetCurrentDirectory(), "sql-references.txt");

			using (var outputFile = new StreamWriter(pathToFile)) {
				foreach (var referenceMapEntry in referenceMap) {
					var entity = referenceMapEntry.Value;
					outputFile.WriteLine(reportFileSeparator);
					outputFile.WriteLine($"NAME: {entity.Name}");
					outputFile.WriteLine($"Sql Type: {entity.Type}");

					outputFile.WriteLine($"Files referencing: {string.Join(",", entity.FilesReferencing)}");
					outputFile.WriteLine($"Stored Procs referencing: {string.Join(",", entity.ProcsReferencing)}");
				}
			}

			return pathToFile;
		}
	}
}
