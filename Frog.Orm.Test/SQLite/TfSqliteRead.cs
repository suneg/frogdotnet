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
            connection = new SqliteConnection("Data Source=:memory:;version=3");

            var builder = new SchemaBuilder(connection);
            builder.CreateTableFromType<Sample>();
            builder.CreateTableFromType<TypeWithEnumMember>();
            builder.CreateTableFromType<TypeWithGuidPrimaryKey>();
            builder.CreateTableFromType<TypeWithFractionMembers>();

            builder.CreateViewFromType<Sample>("AllSamples");

            var repository = new Repository(connection);
            repository.Create(new Sample());
            repository.Create(new Sample());
            repository.Create(new Sample());

            repository.Create(new TypeWithEnumMember { ActualEnumValue = SampleEnum.B });
            repository.Create(new TypeWithFractionMembers() { DoubleValue = 1.2345678, DecimalValue = (decimal)2.345678 });
        }

        [TearDown]
        public void Teardown()
        {
            connection.Dispose();
        }
    }
}
