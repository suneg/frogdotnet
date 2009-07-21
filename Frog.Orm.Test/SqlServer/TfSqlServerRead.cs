using System;
using System.Data.SqlClient;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace Frog.Orm.Test.SqlServer
{
    [TestFixture, Category("SqlServer")]
    public class TfSqlServerRead : DatabaseReadTests
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
            builder.CreateTableFromType<Sample>();
            builder.CreateTableFromType<TypeWithEnumMember>();
            builder.CreateTableFromType<TypeWithGuidPrimaryKey>();
            builder.CreateViewFromType<Sample>("AllSamples");

            connection = new SqlServerConnection(connectionString);

            var repository = new Repository(connection);
            repository.Create(new Sample());
            repository.Create(new Sample());
            repository.Create(new Sample());

            repository.Create(new TypeWithEnumMember {ActualEnumValue = SampleEnum.B});
            connection.CommitChanges();
        }

        [TearDown]
        public void Teardown()
        {
            builder.RemoveTableFromType<Sample>();
            builder.RemoveTableFromType<TypeWithEnumMember>();
            builder.RemoveTableFromType<TypeWithGuidPrimaryKey>();
            builder.RemoveView("AllSamples");

            setupConnection.Close();
            connection.Dispose();
        }

        [Test, Ignore("No support yet for GUID primary keys")]
        public void GetByGuidPrimaryKey()
        {
            using (connection)
            {
                var repository = new Repository(connection);
                repository.Create(new TypeWithGuidPrimaryKey { PrimaryKey = new Guid("FE38313A-7B56-4b8a-A856-AB7346476DE1") });

                var entity = repository.Get<TypeWithGuidPrimaryKey>(new Guid("FE38313A-7B56-4b8a-A856-AB7346476DE1"));
                Assert.That(entity.PrimaryKey, Is.EqualTo("FE38313A-7B56-4b8a-A856-AB7346476DE1"));
            }
        }
    }
}