using System.Collections.Generic;

namespace DbRefFinder {
	public class ReferenceList {
		public string Name { get; set; }

		public SqlObjectType Type { get; set; }

		public HashSet<string> FilesReferencing { get; set; }

		public HashSet<string> ProcsReferencing { get; set; }

		public ReferenceList(string name, SqlObjectType type) {
			Name = name;
			Type = type;
			FilesReferencing = new HashSet<string>();
			ProcsReferencing = new HashSet<string>();
		}
	}
}
