using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;

namespace DbRefFinder {
	public class DbAccess {
		private const string getAllTableNames = "SELECT [name] FROM sys.tables";
		private const string getAllStoredProcNames = "SELECT [name] FROM dbo.sysobjects WHERE type = 'P'";

		private readonly string connectionString;

		public DbAccess() {
			connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
		}

		/// <summary>
		/// Gets a list of all table names in the DB
		/// </summary>
		/// <returns></returns>
		public IEnumerable<string> GetTableNames() {
			return Get(getAllTableNames);
		}
		
		/// <summary>
		/// Gets a list of all stored proces in the DB
		/// </summary>
		/// <returns></returns>
		public IEnumerable<string> GetStoredProcNames() {
			return Get(getAllStoredProcNames);
		}

		/// <summary>
		/// Gets all stored procs referencing a given table/proc
		/// </summary>
		/// <param name="name">Name of table/proc you're checking</param>
		/// <returns></returns>
		public IEnumerable<string> FindRefsFromStoredProcs(string name) {
			var sqlQuery = $@"SELECT Name FROM sys.procedures WHERE OBJECT_DEFINITION(OBJECT_ID) LIKE '%{name}%'";

			return Get(sqlQuery);
		}

		private IEnumerable<string> Get(string sqlQuery) {
			using (var connection = new SqlConnection(connectionString)) {;
				using (var command = new SqlCommand(sqlQuery, connection)) {
					using (var reader = command.ExecuteReader()) {
						while (reader.Read()) {
							yield return reader.GetString(0);
						}
					}
				}
			}
		}
	}
}
