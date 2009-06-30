using System.Collections.Generic;
using System.Data.Common;
using System.Data.SQLite;
using System.Linq;
using Frog.Orm.Conditions;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace Frog.Orm.Test
{
    [TestFixture]
    public class TfCustomRepositories
    {
        private IConnection connection;

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
            builder.CreateViewFromType<Sample>("AllSamples");

            connection = new SqliteConnection((SQLiteConnection)memoryDbConnection);

            var repository = new Repository(connection);
            repository.Create(new Sample());
            repository.Create(new Sample());
            repository.Create(new Sample());

            repository.CommitChanges();
        }

        private static DbConnection GetInMemorySqliteConnection()
        {
            var sqliteConnection = new SQLiteConnection("Data Source=:memory:;version=3");
            sqliteConnection.Open();

            return sqliteConnection;
        }

        [Test]
        public void GetManyFromView()
        {
            using (var repository = new TestRepository(connection))
            {
                var samples = repository.GetMany<Sample>("AllSamples", null);
                Assert.That(samples.Count(), Is.EqualTo(3));
            }
        }

        [Test]
        public void GetSingleFromView()
        {
            using (var repository = new TestRepository(connection))
            {
                var sample = repository.GetSingle<Sample>("AllSamples", null);
                Assert.That(sample.Id, Is.EqualTo(1));
            }
        }
    }

    internal class TestRepository : Repository
    {
        public TestRepository(IConnection connection) : base(connection)
        {
            
        }

        public new IEnumerable<T> GetMany<T>(string sourceName, ICondition condition)
        {
            return base.GetMany<T>(sourceName, condition);
        }

        public new T GetSingle<T>(string sourceName, ICondition condition)
        {
            return base.GetSingle<T>(sourceName, condition);
        }
    }
}
