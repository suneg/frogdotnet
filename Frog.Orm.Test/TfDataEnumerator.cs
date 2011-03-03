using System;
using System.Collections.Generic;
using System.Data;
using Frog.Orm.Conditions;
using NUnit.Framework;
using System.Linq;
using Is=NUnit.Framework.Is;

namespace Frog.Orm.Test
{
    [TestFixture]
    public class TfDataEnumerator
    {
        private DataEnumerator enumerator;
        private MockTable table;
        private MockOrmRepository mockRepository;

        [SetUp]
        public void Setup()
        {
            
            mockRepository = new MockOrmRepository();

            enumerator = new DataEnumerator(mockRepository);
            table = new MockTable();
        }

        [Test]
        public void EnumerateTableWithPrimaryKey()
        {
            table.AddColumn("Id", typeof (Int32));
            table.AddRow(5);
            table.AddRow(10);

            var samples = enumerator.GetEnumerator<Sample>(table.GetDataReader());

            var list = samples.ToList();
            Assert.That(list.Count, Is.EqualTo(2));
            
            Assert.That(list[0].Id, Is.EqualTo(5));
            Assert.That(list[1].Id, Is.EqualTo(10));
        }

        [Test]
        public void EnumerateTableWithSingleColumn()
        {
            table.AddColumn("Name", typeof(String));
            table.AddRow("Pink");
            table.AddRow("Floyd");

            var samples = enumerator.GetEnumerator<SingleColumn>(table.GetDataReader());

            var list = samples.ToList();
            Assert.That(list.Count, Is.EqualTo(2));

            Assert.That(list[0].Name, Is.EqualTo("Pink"));
            Assert.That(list[1].Name, Is.EqualTo("Floyd"));
        }

        [Test]
        public void EnumerateRowsWithRequiredDependencyAttributeOnMappingClass()
        {
            table.AddColumn("Name", typeof(String));
            table.AddRow("Edwards W. Deming");
            

            var samples = enumerator.GetEnumerator<SingleColumnWithRequiredDependency>(table.GetDataReader());

            var list = samples.ToList();
            Assert.That(list.Count, Is.EqualTo(1));

            Assert.That(list[0].Name, Is.EqualTo("Edwards W. Deming"));
            Assert.That(list[0].TheSame, Is.EqualTo(mockRepository));
        }

        [Test]
        public void EnumerateRowWithDbNullValue()
        {
            table.AddColumn("Name", typeof(String));
            table.AddColumn("Age", typeof(Int32));
            table.AddRow("Edwards W. Deming", DBNull.Value);

            var samples = enumerator.GetEnumerator<TwoColumns>(table.GetDataReader());

            var list = samples.ToList();
            Assert.That(list.Count, Is.EqualTo(1));

            Assert.That(list[0].Name, Is.EqualTo("Edwards W. Deming"));
            Assert.That(list[0].Age, Is.Null);
        }
    }

    #region Mocked repository 
    
    #endregion

    #region Sample Mapping classes

    [Table]
    internal class SingleColumn
    {
        [Column]
        public string Name { get; set; }
    }

    [Table]
    internal class SingleColumnWithRequiredDependency
    {
        [RequiredDependency]
        private IRepository repository { get; set; }

        [Column]
        public string Name { get; set; }

        public IRepository TheSame { set { repository = value; }get { return repository; } }

    }

    [Table]
    internal class TwoColumns
    {
        [Column]
        public string Name { get; set; }

        [Column]
        public int? Age { get; set; }
    }

    #endregion

    #region Helper mocks

    internal class MockTable
    {
        private readonly DataTable dataTable;

        public MockTable()
        {
            dataTable = new DataTable();
        }

        public void AddColumn(string name, Type type)
        {
            dataTable.Columns.Add(new DataColumn(name, type));
        }

        public void AddRow(params object[] values)
        {
            var row = dataTable.NewRow();
            row.ItemArray = values;

            dataTable.Rows.Add(row);
        }

        public IDataReader GetDataReader()
        {
            return new DataTableReader(dataTable);
        }
    }

    internal class MockOrmRepository : IRepository
    {
        public void Dispose()
        {
            throw new System.NotImplementedException();
        }

        public T Get<T>(long primaryKeyValue)
        {
            throw new System.NotImplementedException();
        }

        public T Get<T>(Guid primaryKeyValue)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<T> GetAll<T>()
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<T> GetWhere<T>(ICondition condition)
        {
            throw new System.NotImplementedException();
        }

        public void CommitChanges()
        {
            throw new System.NotImplementedException();
        }

        public T Create<T>(T obj)
        {
            throw new System.NotImplementedException();
        }

        public void CreateFast<T>(T obj)
        {
            throw new System.NotImplementedException();
        }

        public void Remove(object obj)
        {
            throw new System.NotImplementedException();
        }

        public void RemoveAll<T>()
        {
            throw new System.NotImplementedException();
        }

        public void RemoveWhere<T>(ICondition condition)
        {
            throw new System.NotImplementedException();
        }

        public void Update(object obj)
        {
            throw new System.NotImplementedException();
        }
    }

    #endregion
}
