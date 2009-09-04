using System.Data.SqlClient;
using NUnit.Framework;

namespace Frog.Orm.Test.SqlServer
{
    [TestFixture, Category("SQLite")]
    public class TfSqlServerWrite : DatabaseWriteTests
    {
        private SchemaBuilder builder;
        private const string connectionString = @"Data Source=(local)\sqlexpress;initial catalog=frog;integrated security=sspi;";

        [SetUp]
        public void Setup()
        {
            connection = new SqlServerConnection(connectionString);

            builder = new SchemaBuilder(connection);
            builder.CreateTableFromType<Entity>();
            builder.CreateTableFromType<TypeWithEnumMember>();
            builder.CreateTableFromType<TypeWithBoolean>();
        }

        [TearDown]
        public void Teardown()
        {
            builder.RemoveTableFromType<Entity>();
            builder.RemoveTableFromType<TypeWithEnumMember>();
            builder.RemoveTableFromType<TypeWithBoolean>();

            connection.CommitChanges();
            connection.Dispose();
        }

        [Test, Explicit]
        public void Insert25000Rows()
        {
            var repository = new Repository(connection);

            for (int i = 0; i < 25000; i++)
            {
                var instance = new Entity();
                instance.Text = "Hello world " + i;
                repository.Create(instance);
            }
        }
    }
}
