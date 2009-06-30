using System.Data;
using Frog.Orm.Conditions;
using Frog.Orm.Dialects;
using Frog.Orm.Test;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rhino.Mocks;
using System.Linq;

namespace Frog.Orm.Test
{
    [TestFixture]
    public class TfTransaction
    {
        private MockRepository mocks;
        private IDbTransaction dbTransaction;
        private IDbConnection connection;
        private IDbCommand command;
        private IDataEnumerator enumerator;
        private IDataReader reader;

        [SetUp]
        public void Setup()
        {
            mocks = new MockRepository();
            dbTransaction = mocks.StrictMock<IDbTransaction>();
            connection = mocks.StrictMock<IDbConnection>();
            command = mocks.StrictMock<IDbCommand>();
            enumerator = mocks.StrictMock<IDataEnumerator>();
            reader = mocks.StrictMock<IDataReader>();

            Expect.Call(dbTransaction.Connection).Return(connection);
            Expect.Call(connection.CreateCommand()).Return(command);
            Expect.Call(command.Transaction).SetPropertyWithArgument(dbTransaction);
        }

        // TODO: Add test that uses GetWhere()

        // TODO: Add test that uses Update()

        [Test]
        public void ExecuteGetAll()
        {
            Expect.Call(command.CommandText).SetPropertyWithArgument("SELECT [Id] FROM [Sample]");
            Expect.Call(enumerator.GetEnumerator<Sample>(null))
                .Return(new[]
                            {
                                new Sample(), new Sample()
                            });
            LastCall.IgnoreArguments();

            mocks.ReplayAll();

            var transaction = new Transaction(dbTransaction, new TransactSqlDialect(), enumerator);
            var all = transaction.GetAll<Sample>();

            Assert.That(all.Count(), Is.EqualTo(2));
        }

        [Test]
        public void ExecuteGetByPrimaryKey()
        {
            Expect.Call(command.CommandText).SetPropertyWithArgument("SELECT [Id] FROM [Sample] WHERE ([Id] = 7)");
            Expect.Call(command.ExecuteReader()).Return(reader);

            Expect.Call(enumerator.GetEnumerator<Sample>(reader))
                .Return(new[]
                            {
                                new Sample { Id = 7 }
                            });

            mocks.ReplayAll();

            var transaction = new Transaction(dbTransaction, new TransactSqlDialect(), enumerator);
            var match = transaction.GetByPrimaryKey<Sample>(7);

            Assert.That(match.Id, NUnit.Framework.SyntaxHelpers.Is.EqualTo(7));
        }

        [Test]
        public void StoreNewInstance()
        {
            var person = new Person {Name = "Sune"};

            Expect.Call(command.CommandText).SetPropertyWithArgument("INSERT INTO [People]([Name]) VALUES('Sune')");
            Expect.Call(command.ExecuteNonQuery()).Return(1);

            Expect.Call(connection.CreateCommand()).Return(command);
            Expect.Call(dbTransaction.Connection).Return(connection);
            Expect.Call(command.CommandText).SetPropertyWithArgument("SELECT @@IDENTITY");
            Expect.Call(command.Transaction).SetPropertyWithArgument(dbTransaction);
            Expect.Call(command.ExecuteScalar()).Return(14);

            mocks.ReplayAll();

            var transaction = new Transaction(dbTransaction, new TransactSqlDialect(), enumerator);
            var created = transaction.Create(person);

            Assert.That(created.Name, Is.EqualTo("Sune"));
            Assert.That(created.Id, Is.EqualTo(14));
        }

        [Test, ExpectedException(ExceptionType = typeof(InvalidRowCountException))]
        public void StoreNewInstanceFails()
        {
            var person = new Person { Name = "Sune" };

            Expect.Call(command.CommandText).SetPropertyWithArgument("INSERT INTO [People]([Name]) VALUES('Sune')");
            Expect.Call(command.ExecuteNonQuery()).Return(0);

            mocks.ReplayAll();
            var transaction = new Transaction(dbTransaction, new TransactSqlDialect(), enumerator);
            transaction.Create(person);
        }

        [Test]
        public void DeleteInstance()
        {
            var person = new Person {Id = 5, Name = "Sune"};

            Expect.Call(command.CommandText).SetPropertyWithArgument("DELETE FROM [People] WHERE ([Id] = 5)");
            Expect.Call(command.ExecuteNonQuery()).Return(1);

            mocks.ReplayAll();

            var transaction = new Transaction(dbTransaction, new TransactSqlDialect(), enumerator);
            transaction.Delete(person);
        }

        [Test, ExpectedException(ExceptionType = typeof(InvalidRowCountException))]
        public void DeleteMissingInstanceFails()
        {
            var person = new Person {Id = 5, Name = "Sune"};

            Expect.Call(command.CommandText).SetPropertyWithArgument("DELETE FROM [People] WHERE ([Id] = 5)");
            Expect.Call(command.ExecuteNonQuery()).Return(0);

            mocks.ReplayAll();

            var transaction = new Transaction(dbTransaction, new TransactSqlDialect(), enumerator);
            transaction.Delete(person);
        }

        [Test, ExpectedException(ExceptionType = typeof(InvalidRowCountException))]
        public void UpdateMissingInstanceFails()
        {
            var person = new Person { Id = 5, Name = "Sune" };

            Expect.Call(command.CommandText).SetPropertyWithArgument("UPDATE [People] SET [Name] = 'Sune' WHERE ([Id] = 5)");
            Expect.Call(command.ExecuteNonQuery()).Return(0);

            mocks.ReplayAll();

            var transaction = new Transaction(dbTransaction, new TransactSqlDialect(), enumerator);
            transaction.Update(person);
        }

        // TODO: Add test that uses DeleteWhere()

        [Test]
        public void DeleteWhere()
        {
            Expect.Call(command.CommandText).SetPropertyWithArgument("DELETE FROM [People] WHERE ([Id] = 42)");
            Expect.Call(command.ExecuteNonQuery()).Return(1);

            mocks.ReplayAll();

            var transaction = new Transaction(dbTransaction, new TransactSqlDialect(), enumerator);
            transaction.DeleteWhere<Person>(Field.Equals("Id", 42));
        }

        [Test]
        public void DeleteAll()
        {
            Expect.Call(command.CommandText).SetPropertyWithArgument("DELETE FROM [People]");
            Expect.Call(command.ExecuteNonQuery()).Return(1);

            mocks.ReplayAll();

            var transaction = new Transaction(dbTransaction, new TransactSqlDialect(), enumerator);
            transaction.DeleteAll<Person>();
        }

        [TearDown]
        public void Teardown()
        {
            mocks.VerifyAll();
        }
    }

    #region Sample mapping classes

    [Table(Name="People")]
    public class Person
    {
        [PrimaryKey]
        public int Id { get; set; }

        [Column]
        public string Name { get; set; }
    }

    #endregion
}
