using System.Collections.Generic;
using System.Linq;
using Frog.Orm.Conditions;
using NUnit.Framework;

namespace Frog.Orm.Test
{
    [TestFixture]
    public class TfCustomRepositories
    {
        private IConnection connection;

        [SetUp]
        public void Setup()
        {
            connection = new SqliteConnection("Data Source=:memory:;version=3");

            var builder = new SchemaBuilder(connection);
            builder.CreateTableFromType<Sample>();
            builder.CreateViewFromType<Sample>("AllSamples");

            var repository = new Repository(connection);
            repository.Create(new Sample());
            repository.Create(new Sample());
            repository.Create(new Sample());

            connection.CommitChanges();
        }

        [Test]
        public void GetManyFromView()
        {
            using (connection)
            {
                var repository = new TestRepository(connection);
                var samples = repository.GetMany<Sample>("AllSamples", null);
                Assert.That(samples.Count(), Is.EqualTo(3));
            }
        }

        [Test]
        public void GetSingleFromView()
        {
            using (connection)
            {
                var repository = new TestRepository(connection);
                var sample = repository.GetSingle<Sample>("AllSamples", null);
                Assert.That(sample.Id, Is.EqualTo(1));
            }
        }

        [Test, Description("If a custom repository did not provide an IRepository for initializing the DataEnumerator on the connection, problems would occur. Fixed by moving initialization to BaseRepository")]
        public void TestDefectNonAutoInitOfDataEnumerator()
        {
            using (connection)
            {
                var repository = new TestRepository(connection);

                repository.Create<Sample>("test", 5);

                var res = repository.Search("test").Select(p => p.Id).ToArray();
                Assert.That(res, Is.EqualTo(new long[] { 1, 2, 3, 4 }));

            }
        }
    }

    internal class TestRepository : BaseRepository
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

        public Sample Create<T>(string a, int asdf)
        {
            return Create<Sample>(new Sample());
        }

        public IEnumerable<Sample> Search(string searchPattern)
        {
            return base.GetAll<Sample>();
        }
    }

    
}
