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
            connection = new SqliteConnection("Data Source=:memory:;version=3");

            var builder = new SchemaBuilder(connection);
            builder.CreateTableFromType<Entity>();
            builder.CreateTableFromType<TypeWithEnumMember>();
            builder.CreateTableFromType<TypeWithBoolean>();
        }

        [TearDown]
        public void Teardown()
        {
            connection.Dispose();
        }
    }
}
