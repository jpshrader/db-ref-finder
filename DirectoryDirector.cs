﻿using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DbRefFinder {
	public class DirectoryDirector {
		public void Scour(string directoryPath, Dictionary<string, ReferenceList> referenceMap) {
			foreach (var filePath in Directory.GetFiles(directoryPath)) {
				ScourFile(filePath, referenceMap);
			}

			foreach (var directory in Directory.GetDirectories(directoryPath)) {
				Scour(directory, referenceMap);
			}
		}

		private void ScourFile(string filePath, Dictionary<string, ReferenceList> referenceMap) {
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