using System.Data.Common;
using System.Data.SQLite;
using NUnit.Framework;

namespace Frog.Orm.Test.SQLite
{
    [TestFixture, Category("SQLite")]
    public class TfSqliteRead : DatabaseReadTests
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
            builder.CreateTableFromType<Sample>();
            builder.CreateTableFromType<TypeWithEnumMember>();
            builder.CreateTableFromType<TypeWithGuidPrimaryKey>();
            builder.CreateViewFromType<Sample>("AllSamples");

            connection = new SqliteConnection((SQLiteConnection)memoryDbConnection);

            var repository = new Repository(connection);
            repository.Create(new Sample());
            repository.Create(new Sample());
            repository.Create(new Sample());

            repository.Create(new TypeWithEnumMember { ActualEnumValue = SampleEnum.B });
        }

        private static DbConnection GetInMemorySqliteConnection()
        {
            var sqliteConnection = new SQLiteConnection("Data Source=:memory:;version=3");
            sqliteConnection.Open();

            return sqliteConnection;
        }
    }
}
