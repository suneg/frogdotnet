using System.Data.SqlClient;
using NUnit.Framework;

namespace Frog.Orm.Test.SqlServer
{
    [TestFixture, Category("SQLite")]
    public class TfSqlServerWrite : DatabaseWriteTests
    {
        private SchemaBuilder builder;
        private SqlConnection setupConnection;
        private const string connectionString = @"Data Source=(local)\sqlexpress;initial catalog=frog;integrated security=sspi;";

        [TestFixtureSetUp]
        public void Initialize()
        {
            Configuration.Initialize("Frog.Orm.Test.dll.config");
        }

        [SetUp]
        public void Setup()
        {
            setupConnection = new SqlConnection(connectionString);

            builder = new SchemaBuilder(setupConnection);
            builder.CreateTableFromType<Entity>();
            builder.CreateTableFromType<TypeWithEnumMember>();
            builder.CreateTableFromType<TypeWithBoolean>();

            connection = new SqlServerConnection(connectionString);
        }

        [TearDown]
        public void Teardown()
        {
            builder.RemoveTableFromType<Entity>();
            builder.RemoveTableFromType<TypeWithEnumMember>();
            builder.RemoveTableFromType<TypeWithBoolean>();

            setupConnection.Close();
            connection.Dispose();
        }
    }
}
