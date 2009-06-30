using System;
using System.Linq;
using Frog.Orm.Conditions;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace Frog.Orm.Test
{
    public abstract class DatabaseReadTests
    {
        protected IConnection connection;

        [Test]
        public void GetByIntegerPrimaryKey()
        {
            using(var repository = new Repository(connection))
            {
                var sample = repository.Get<Sample>(2);
                Assert.That(sample.Id, Is.EqualTo(2));
            }
        }

        [Test]
        public void GetAll()
        {
            using(var repository = new Repository(connection))
            {
                var all = repository.GetAll<Sample>();
                Assert.That(all.Count(), Is.EqualTo(3));
            }
        }

        [Test, Description("Using JustInTimeDataReader. Designed to catch 'failed: System.InvalidOperationException : There is already an open DataReader associated with this Command which must be closed first.'")]
        public void GetAllButDontEnumerate()
        {
            using (var repository = new Repository(connection))
            {
                var all = repository.GetAll<Sample>();
            }
        }

        [Test]
        public void FetchTwiceUsingSameTransaction()
        {
            using(var repository = new Repository(connection))
            {
                repository.Get<Sample>(1);
                repository.Get<Sample>(2);
            }
        }

        [Test]
        public void GetWithSpecialCondition()
        {
            using(var repository = new Repository(connection))
            {                
                var samples = repository.GetWhere<Sample>(Field.Equals("Id", 3));
                Assert.That(samples.Count(), Is.EqualTo(1));
            }
        }

        [Test]
        public void GetTypeWithEnum()
        {
            using(var repository = new Repository(connection))
            {
                var entities = repository.GetAll<TypeWithEnumMember>();
                Assert.That(entities.First().ActualEnumValue, Is.EqualTo(SampleEnum.B));
            }
        }
    }


    internal enum SampleEnum { A, B, C }

    [Table]
    internal class TypeWithGuidPrimaryKey
    {
        [PrimaryKey]
        public Guid PrimaryKey { get; set; }
    }

    [Table]
    internal class TypeWithEnumMember
    {
        [Column]
        public SampleEnum ActualEnumValue { get; set; }
    }

    [Table]
    internal class Sample
    {
        [PrimaryKey]
        public long Id { get; set; }
    }
}