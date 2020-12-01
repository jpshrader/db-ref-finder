using System;
using System.Collections.Generic;

namespace DbRefFinder {
	public class SqlProcInspector {
		public void Inspect(DbAccess dbAccess, Dictionary<string, ReferenceList> referenceMap) {
			SqlObjectType? objectType = null;
			var count = 0;
			var totalCount = referenceMap.Keys.Count;

			foreach(var entity in referenceMap.Keys) {
				var referenceList = referenceMap[entity];
				
				if (objectType != referenceList.Type) {
					objectType = referenceList.Type;
					Console.WriteLine($"Processing {objectType}s...");
				}

				if (++count % 10 == 0) {
					Console.WriteLine($"{count}/{totalCount} processed");
				}

				var referencingProcs = dbAccess.FindRefsFromStoredProcs(entity);
				foreach(var referencingProc in referencingProcs) {
					referenceList.ProcsReferencing.Add(referencingProc);
				}
			}
		}
	}
}
