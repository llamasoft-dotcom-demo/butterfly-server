using Butterfly.Core.Database;
using Butterfly.Core.Test;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;
using System.Threading.Tasks;

namespace Butterfly.SqlServer.Test
{
	[TestClass]
	public class SqlServerTest
	{
		const string connectionString = @"Server=localhost; Initial Catalog=Butterfly; User ID=sa; Password=f00b@rF!ght3rs";
		const string dbSql = "Butterfly.SqlServer.Test.db.sql";

		[TestMethod]
		public async Task TestDatabase()
		{
			IDatabase database = new SqlServerDatabase(connectionString);
			await DatabaseUnitTest.TestDatabase(database, Assembly.GetExecutingAssembly(), dbSql);
		}

		[TestMethod]
		public async Task TestDynamic()
		{
			var database = new SqlServerDatabase(connectionString);
			await DynamicUnitTest.TestDatabase(database, Assembly.GetExecutingAssembly(), dbSql);
		}
	}
}
