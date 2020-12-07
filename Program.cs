using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;

namespace DbRefFinder {
	public static class Program {
		private static readonly ConcurrentDictionary<string, ReferenceList> sqlReferenceMap = new ConcurrentDictionary<string, ReferenceList>();

		private static readonly DbAccess dbAccess = new DbAccess();
		private static readonly string reportFileSeparator = new string('=', 50);

		private static string directoryToSearch;

		public static void Main(string[] args) {
			directoryToSearch = ConfigurationManager.AppSettings["DirectoryToSearch"].ToString();

			var tableNames = dbAccess.GetTableNames();
			AddToReferenceMap(tableNames, SqlObjectType.Table);
			var storedProcNames = dbAccess.GetStoredProcNames();
			AddToReferenceMap(storedProcNames, SqlObjectType.StoredProcedure);

			Console.WriteLine("Processing Local Files...");
			DirectoryDirector.Scour(directoryToSearch, sqlReferenceMap);
			SqlProcInspector.Inspect(dbAccess, sqlReferenceMap);

			var outputPath = WriteOutputFile(sqlReferenceMap);
			Console.WriteLine($"File written to: {outputPath}");
			Console.ReadKey();
		}

		private static void AddToReferenceMap(IEnumerable<string> sqlEntityNames, SqlObjectType systemType) {
			foreach (var sqlEntityName in sqlEntityNames) {
				sqlReferenceMap.TryAdd(sqlEntityName, new ReferenceList(sqlEntityName, systemType));
			}
		}

		private static string WriteOutputFile(ConcurrentDictionary<string, ReferenceList> referenceMap) {
			var pathToFile = Path.Combine(Directory.GetCurrentDirectory(), "sql-references.txt");

			using (var outputFile = new StreamWriter(pathToFile, append: false)) {
				var itemsWithNoRefs = referenceMap.Values.Where(r => r.FilesReferencing.Count == 0 && r.ProcsReferencing.Count == 0).Select(r => r.Name);
				outputFile.WriteLine($"Items with no references: {string.Join(", ", itemsWithNoRefs)}");

				foreach (var referenceMapEntry in referenceMap) {
					var entity = referenceMapEntry.Value;
					outputFile.WriteLine(reportFileSeparator);
					outputFile.WriteLine($"Name: {entity.Name}");
					outputFile.WriteLine($"Sql Type: {entity.Type}");

					outputFile.WriteLine($"Files referencing: {string.Join(", ", entity.FilesReferencing)}");
					outputFile.WriteLine($"Stored Procs referencing: {string.Join(", ", entity.ProcsReferencing)}");
				}
			}

			return pathToFile;
		}
	}
}
