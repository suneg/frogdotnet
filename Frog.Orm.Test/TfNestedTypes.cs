using System.Collections.Generic;
using System.Data.Common;
using System.Data.SQLite;
using System.Linq;
using Frog.Orm.Conditions;
using Frog.Orm.Test;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace Frog.Orm.Test
{
    [TestFixture]
    public class TfNestedTypes
    {
        private IConnection connection;

        [SetUp]
        public void Setup()
        {
            var memoryDbConnection = GetInMemorySqliteConnection();

            var builder = new SchemaBuilder(memoryDbConnection);
            builder.CreateTableFromType<OrderLine>();
            builder.CreateTableFromType<Order>();

            connection = new SqliteConnection((SQLiteConnection)memoryDbConnection);

            var repository = new Repository(connection);
            repository.Create(new Order());
            repository.Create(new Order());

            var line1 = new OrderLine { Description = "Product 1", ParentId = 1 };
            var line2 = new OrderLine {Description = "Product 2", ParentId = 1 };
            var line3 = new OrderLine { Description = "Product 3", ParentId = 2 };

            repository.Create(line1);
            repository.Create(line2);
            repository.Create(line3);

            
        }

        private static DbConnection GetInMemorySqliteConnection()
        {
            var sqliteConnection = new SQLiteConnection("Data Source=:memory:;version=3");
            sqliteConnection.Open();

            return sqliteConnection;
        }

        [Test]
        public void GetOrderAndOrderlines()
        {
            using (var repository = new Repository(connection))
            {
                var order = repository.Get<Order>(1);

                Assert.That(order.Orderlines.Count(), NUnit.Framework.SyntaxHelpers.Is.EqualTo(2));
            }
        }
    }

    #region Sample mapping classes

    [Table]
    internal class Order
    {
        [PrimaryKey]
        public int Id { get; set; }

        [RequiredDependency]
        public IRepository Repository { get; set; }

        [Column]
        public decimal TotalAmount
        {
            get; set;
        }

        public IEnumerable<OrderLine> Orderlines
        {
            get 
            { 
                return Repository.GetWhere<OrderLine>(Field.Equals("ParentId", this.Id)); 
            }
        }
    }

    [Table]
    internal class OrderLine
    {
        [PrimaryKey]
        public int Id { get; set; }

        [Column]
        public string Description { get; set; }

        [Column]
        public int ParentId { get; set; }
    }

    #endregion
}
