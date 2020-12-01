using System.Collections.Generic;

namespace DbRefFinder {
	public class ReferenceList {
		public string Name { get; set; }

		public SqlObjectType Type { get; set; }

		public List<string> FilesReferencing { get; set; }

		public List<string> ProcsReferencing { get; set; }

		public ReferenceList(string name, SqlObjectType type) {
			Name = name;
			Type = type;
			FilesReferencing = new List<string>();
			ProcsReferencing = new List<string>();
		}
	}
}
