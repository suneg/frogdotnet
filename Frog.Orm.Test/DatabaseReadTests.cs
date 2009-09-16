using System;
using System.Linq;
using Frog.Orm.Syntax;
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
            var repository = new Repository(connection);
            var sample = repository.Get<Sample>(2);
            Assert.That(sample.Id, Is.EqualTo(2));
        }

        [Test]
        public void GetAll()
        {
            var repository = new Repository(connection);
            var all = repository.GetAll<Sample>();
            Assert.That(all.Count(), Is.EqualTo(3));
        }

        [Test, Description("Using JustInTimeDataReader. Designed to catch 'failed: System.InvalidOperationException : There is already an open DataReader associated with this Command which must be closed first.'")]
        public void GetAllButDontEnumerate()
        {
            var repository = new Repository(connection);
            var all = repository.GetAll<Sample>();
        }

        [Test]
        public void FetchTwiceUsingSameTransaction()
        {
            var repository = new Repository(connection);
            repository.Get<Sample>(1);
            repository.Get<Sample>(2);
        }

        [Test]
        public void GetWithSpecialCondition()
        {
            var repository = new Repository(connection);
            var samples = repository.GetWhere<Sample>(Field.Equals("Id", 3));
            Assert.That(samples.Count(), Is.EqualTo(1));
        }

        [Test]
        public void GetTypeWithEnum()
        {
            var repository = new Repository(connection);
            var entities = repository.GetAll<TypeWithEnumMember>();
            Assert.That(entities.First().ActualEnumValue, Is.EqualTo(SampleEnum.B));
        }

        [Test]
        public void GetTypeWithFractionTypes()
        {
            var repository = new Repository(connection);
            var entities = repository.GetAll<TypeWithFractionMembers>();
            var first = entities.First();
            Assert.That(first.DoubleValue, Is.EqualTo(1.2345678));
            //Assert.That(first.DecimalValue, Is.EqualTo(2.345678));    // TODO: Can't get decimals to work with Sqlite
        }

        [Test, Ignore("Not possible yet - We need a working solution for this issue")]
        public void IterateResultTwice()
        {
            var repository = new Repository(connection);
            var entities = repository.GetAll<TypeWithEnumMember>();

            Assert.That(entities.Count(), Is.EqualTo(1));
            Assert.That(entities.Count(), Is.EqualTo(1));
        }

        [Test]
        public void CountEntitiesUsingScalarResult()
        {
            var repository = new CustomRepositoryForScalarQueries(connection);
            Assert.That(repository.CountEntities(), Is.EqualTo(3));
        }

        [Test]
        public void AverageFieldUsingScalarResult()
        {
            var repository = new CustomRepositoryForScalarQueries(connection);
            Assert.That(repository.GetAverage(), Is.EqualTo(2));
        }

        [Test]
        public void SumFieldUsingScalarResult()
        {
            var repository = new CustomRepositoryForScalarQueries(connection);
            Assert.That(repository.GetSum(), Is.EqualTo(6));
        }
    }

    internal class CustomRepositoryForScalarQueries : Repository
    {
        public CustomRepositoryForScalarQueries(IConnection connection) : base(connection)
        {
            
        }

        public int CountEntities()
        {
            return Convert.ToInt32(GetScalar(Scalar.Count("Sample")));
        }

        public double GetAverage()
        {
            return Convert.ToDouble(GetScalar(Scalar.Average("Id","Sample")));
        }

        public int GetSum()
        {
            return Convert.ToInt32(GetScalar(Scalar.Sum("Id", "Sample")));
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
    internal class TypeWithFractionMembers
    {
        [Column]
        public double DoubleValue { get; set; }

        [Column]
        public decimal DecimalValue { get; set; }
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