using System.Collections.Generic;

namespace DbRefFinder {
	public class SqlProcInspector {
		public void Inspect(DbAccess dbAccess, Dictionary<string, ReferenceList> referenceMap) {
			foreach(var entity in referenceMap.Keys) {
				var referencingProcs = dbAccess.FindRefsFromStoredProcs(entity);
				referenceMap[entity].ProcsReferencing.AddRange(referencingProcs);
			}
		}
	}
}
