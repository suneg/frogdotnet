using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using Frog.Orm.Syntax;
using Frog.Orm.Test;
using NUnit.Framework;

namespace Frog.Orm.Test
{
    [TestFixture]
    public class TfNestedTypes
    {
        private IConnection connection;

        [SetUp]
        public void Setup()
        {
            connection = new SqliteConnection("Data Source=:memory:;version=3");

            var builder = new SchemaBuilder(connection);
            builder.CreateTableFromType<OrderLine>();
            builder.CreateTableFromType<WebsiteOrder>();

            var repository = new Repository(connection);
            repository.Create(new WebsiteOrder());
            repository.Create(new WebsiteOrder());

            var line1 = new OrderLine { Description = "Product 1", ParentId = 1 };
            var line2 = new OrderLine {Description = "Product 2", ParentId = 1 };
            var line3 = new OrderLine { Description = "Product 3", ParentId = 2 };

            repository.Create(line1);
            repository.Create(line2);
            repository.Create(line3);

            
        }

        [Test]
        public void GetOrderAndOrderlines()
        {
            using (connection)
            {
                var repository = new Repository(connection);
                var order = repository.Get<WebsiteOrder>(1);

                Assert.That(order.Orderlines.Count(), Is.EqualTo(2));
            }
        }
    }

    #region Sample mapping classes

    [Table]
    internal class WebsiteOrder
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
