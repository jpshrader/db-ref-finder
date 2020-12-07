using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DbRefFinder {
	public static class DirectoryDirector {
		public static void Scour(string directoryPath, ConcurrentDictionary<string, ReferenceList> referenceMap) {
			var directoryFiles = Directory.GetFiles(directoryPath);
			Console.WriteLine($"Processing: {directoryPath} ({directoryFiles.Length} file(s))");

			Parallel.ForEach(directoryFiles, filePath => {
				ScourFile(filePath, referenceMap);
			});

			Parallel.ForEach(Directory.GetDirectories(directoryPath), directory => {
				Scour(directory, referenceMap);
			});
		}

		private static void ScourFile(string filePath, ConcurrentDictionary<string, ReferenceList> referenceMap) {
			string line;
			var file = new StreamReader(filePath);

			while ((line = file.ReadLine()) != null) {
				var referencedEntities = referenceMap.Keys.Where(r => line.Contains(r));
				foreach(var entity in referencedEntities) {
					referenceMap[entity].FilesReferencing.Add(filePath);
				}
			}
			file.Close();
		}
	}
}
