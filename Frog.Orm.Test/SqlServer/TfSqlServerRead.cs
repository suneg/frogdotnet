using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace Frog.Orm.Test.SqlServer
{
    [TestFixture, Category("SqlServer")]
    public class TfSqlServerRead : DatabaseReadTests
    {
        private SchemaBuilder builder;
        private const string connectionString = @"Data Source=(local)\sqlexpress;initial catalog=frog;integrated security=sspi;";

        [TestFixtureSetUp]
        public void Initialize()
        {
            Configuration.Initialize("Frog.Orm.Test.dll.config");
        }

        [SetUp]
        public void Setup()
        {
            connection = new SqlServerConnection(connectionString);

            builder = new SchemaBuilder(connection);
            builder.CreateTableFromType<Sample>();
            builder.CreateTableFromType<TypeWithEnumMember>();
            builder.CreateTableFromType<TypeWithGuidPrimaryKey>();
            builder.CreateTableFromType<TypeWithFractionMembers>();
            builder.CreateViewFromType<Sample>("AllSamples");

            var repository = new Repository(connection);
            repository.Create(new Sample());
            repository.Create(new Sample());
            repository.Create(new Sample());

            repository.Create(new TypeWithEnumMember {ActualEnumValue = SampleEnum.B});
            repository.Create(new TypeWithFractionMembers() {DoubleValue = 1.2345678, DecimalValue = (decimal)2.345678 });
        }

        [TearDown]
        public void Teardown()
        {
            connection.Rollback();
            connection.Dispose();
        }

        [Test, Ignore("No support yet for GUID primary keys")]
        public void GetByGuidPrimaryKey()
        {
            var repository = new Repository(connection);
            repository.Create(new TypeWithGuidPrimaryKey { PrimaryKey = new Guid("FE38313A-7B56-4b8a-A856-AB7346476DE1") });

            var entity = repository.Get<TypeWithGuidPrimaryKey>(new Guid("FE38313A-7B56-4b8a-A856-AB7346476DE1"));
            Assert.That(entity.PrimaryKey, Is.EqualTo("FE38313A-7B56-4b8a-A856-AB7346476DE1"));
        }
    }
}