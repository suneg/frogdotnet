using System.Data.Common;
using System.Data.SQLite;
using NUnit.Framework;

namespace Frog.Orm.Test.SQLite
{
    [TestFixture, Category("SQLite")]
    public class TfSqliteWrite : DatabaseWriteTests
    {
        [TestFixtureSetUp]
        public void Initialize()
        {
            Configuration.Initialize("Frog.Orm.Test.dll.config");
        }

        [SetUp]
        public void Setup()
        {
            var memoryDbConnection = GetInMemorySqliteConnection();

            var builder = new SchemaBuilder(memoryDbConnection);
            builder.CreateTableFromType<Entity>();
            builder.CreateTableFromType<TypeWithEnumMember>();
            builder.CreateTableFromType<TypeWithBoolean>();

            connection = new SqliteConnection((SQLiteConnection)memoryDbConnection);
        }

        private static DbConnection GetInMemorySqliteConnection()
        {
            var sqliteConnection = new SQLiteConnection("Data Source=:memory:;version=3");
            sqliteConnection.Open();

            return sqliteConnection;
        }
    }
}
