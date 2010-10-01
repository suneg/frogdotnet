using System;
using System.Collections.Generic;
using Frog.Orm.Conditions;
using Frog.Orm.Dialects;
using Frog.Orm.Syntax;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace Frog.Orm.Test
{
    [TestFixture]
    public class TfTransactSqlDialect
    {
        private TransactSqlDialect factory;

        [SetUp]
        public void Setup()
        {
            factory = new TransactSqlDialect();
        }

        [Test]
        public void SelectSingleColumn()
        {
            Assert.That(factory.Select("table", Field.List("column")), Is.EqualTo("SELECT [column] FROM [table]"));
        }

        [Test]
        public void SelectMultipleColumns()
        {
            Assert.That(
                factory.Select("table", Field.List("column1", "column2", "column3")), 
                Is.EqualTo("SELECT [column1],[column2],[column3] FROM [table]"));
        }

        [Test]
        public void SelectWhereIntegerEquals()
        {
            Assert.That(
                factory.SelectWhere("table", Field.List("Name", "Value"), Field.Equals("column", 5)),
                Is.EqualTo("SELECT [Name],[Value] FROM [table] WHERE ([column] = 5)"));
        }

        [Test]
        public void SelectWhereStringEquals()
        {
            Assert.That(
                factory.SelectWhere("table", Field.List("Name", "Value"), Field.Equals("column", "test")),
                Is.EqualTo("SELECT [Name],[Value] FROM [table] WHERE ([column] = 'test')"));
        }

        [Test, Ignore("Not Implemented yet")]
        public void SelectWhereBooleanEquals()
        {

        }

        [Test]
        public void SelectWhereGuidEquals()
        {
            Assert.That(
                factory.SelectWhere("table", Field.List("Name", "Value"), Field.Equals("column", new Guid("48C42461-EF21-41c4-BE4C-4CDFB656C188"))),
                Is.EqualTo("SELECT [Name],[Value] FROM [table] WHERE ([column] = '48c42461-ef21-41c4-be4c-4cdfb656c188')"));
        }

        [Test, Ignore("Not Implemented yet")]
        public void SelectWhereDateTimeEquals()
        {
            
        }

        [Test]
        public void SelectWhereStringIn()
        {
            Assert.That(
                factory.SelectWhere("table", Field.List("Name", "Value"), Field.In("column", new[] {"t1","t2"})),
                Is.EqualTo("SELECT [Name],[Value] FROM [table] WHERE ([column] IN ('t1', 't2'))"));
        }

        [Test]
        public void SelectWhereIntegerIn()
        {
            Assert.That(
                factory.SelectWhere("table", Field.List("Name", "Value"), Field.In("column", new[] { 1L,2L })),
                Is.EqualTo("SELECT [Name],[Value] FROM [table] WHERE ([column] IN (1, 2))"));
        }


        [Test]
        public void SelectWhereColumnStartsWith()
        {
            Assert.That(
                factory.SelectWhere("table", Field.List("Name", "Value"), Field.StartsWith("column", "test")),
                Is.EqualTo("SELECT [Name],[Value] FROM [table] WHERE ([column] LIKE 'test%')"));
        }

        [Test]
        public void SelectWhereColumnEndsWith()
        {
            Assert.That(
                factory.SelectWhere("table", Field.List("Name", "Value"), Field.EndsWith("column", "test")),
                Is.EqualTo("SELECT [Name],[Value] FROM [table] WHERE ([column] LIKE '%test')"));
        }

        [Test]
        public void SelectWhereMultipleConditionsMet()
        {
            Assert.That(
                factory.SelectWhere("table", 
                                    Field.List("Name", "Value"), Field.And(
                                                                     Field.EndsWith("column", "test"),
                                                                     Field.Contains("column2", "abc")
                                                                     )),

                Is.EqualTo("SELECT [Name],[Value] FROM [table] WHERE (([column] LIKE '%test') AND ([column2] LIKE '%abc%'))"));
        }

        [Test]
        public void SelectWhereAnyOfSeveralConditionsIsTrue()
        {
            Assert.That(
                factory.SelectWhere("table", Field.List("Name", "Value"), Field.Or(
                                                                              Field.StartsWith("col", "abc"),
                                                                              Field.EndsWith("col2", "xyz")
                                                                              )),
                Is.EqualTo("SELECT [Name],[Value] FROM [table] WHERE (([col] LIKE 'abc%') OR ([col2] LIKE '%xyz'))"));
        }

        [Test]
        public void SelectWhereColumnGreaterThan()
        {
            Assert.That(
                factory.SelectWhere("table", Field.List("Text"), Field.GreaterThan("column", 42)),
                Is.EqualTo("SELECT [Text] FROM [table] WHERE ([column] > 42)"));
        }

        [Test]
        public void SelectWhereColumnLessThan()
        {
            Assert.That(
                factory.SelectWhere("table", Field.List("Text"), Field.LessThan("column", 42)),
                Is.EqualTo("SELECT [Text] FROM [table] WHERE ([column] < 42)"));
        }

        // TODO: Test Greater than or equal
        // TODO: Test Less than or equal

        [Test]
        public void SelectWhereColumnNotEquals()
        {
            Assert.That(
                factory.SelectWhere("table", Field.List("Text"), Field.Not(Field.Equals("column", 42))),
                Is.EqualTo("SELECT [Text] FROM [table] WHERE (NOT ([column] = 42))"));
        }

        [Test]
        public void SelectAndOrderByColumnAscending()
        {
            Assert.That(
                factory.SelectWhere("table", Field.List("Name", "Value"), Order.By("Size")),
                Is.EqualTo("SELECT [Name],[Value] FROM [table] ORDER BY [Size] ASC"));
        }

        [Test]
        public void SelectAndOrderByColumnDescending()
        {
            Assert.That(
                factory.SelectWhere("table", Field.List("Name", "Value"), Order.By("Size", OrderDirection.Descending)),
                Is.EqualTo("SELECT [Name],[Value] FROM [table] ORDER BY [Size] DESC"));
        }

        // TODO: Test Order by multiple columns

        [Test]
        public void UpdateStringsWhere()
        {
            var collection = new Dictionary<string, object>();
            collection.Add("Name", "John");
            collection.Add("City", "Copenhagen");

            Assert.That(
                factory.UpdateWhere("table", Field.Equals("Id", 7), collection),
                Is.EqualTo("UPDATE [table] SET [Name] = 'John', [City] = 'Copenhagen' WHERE ([Id] = 7)"));
        }

        [Test]
        public void UpdateMixedTypes()
        {
            var collection = new Dictionary<string, object>();
            collection.Add("Name", "John");
            collection.Add("DateOfBirth", DateTime.Parse("1980-12-25"));
            collection.Add("PomodorosCompleted", 101);

            Assert.That(
                factory.UpdateWhere("People", Field.Equals("Id", 7), collection),
                Is.EqualTo("UPDATE [People] SET [Name] = 'John', [DateOfBirth] = '1980-12-25 00:00:00.000', [PomodorosCompleted] = 101 WHERE ([Id] = 7)"));
        }

        [Test]
        public void InsertNewItem()
        {
            var collection = new Dictionary<string, object>();
            collection.Add("Name", "John");
            collection.Add("DateOfBirth", DateTime.Parse("1980-12-25"));
            collection.Add("PomodorosCompleted", 101);

            Assert.That(
                factory.Insert("People", collection),
                Is.EqualTo("INSERT INTO [People]([Name],[DateOfBirth],[PomodorosCompleted]) VALUES('John','1980-12-25 00:00:00.000',101)"));
        }

        [Test]
        public void UpdateWithEscapedCharacters()
        {
            var collection = new Dictionary<string, object>();
            collection.Add("Name", "ABC ^~'*'| // || \\\\ ? +=(){}%&¤#\" <>");

            Assert.That(
                factory.Update("People", collection),
                Is.EqualTo("UPDATE [People] SET [Name] = 'ABC ^~''*''| // || \\\\ ? +=(){}%&¤#\" <>'"));
        }

        [Test]
        public void InsertWithEscapedCharacters()
        {
            var collection = new Dictionary<string, object>();
            collection.Add("Name", "ABC ^~'*'| // || \\\\ ? +=(){}%&¤#\" <>");

            Assert.That(
                factory.Insert("People", collection),
                Is.EqualTo("INSERT INTO [People]([Name]) VALUES('ABC ^~''*''| // || \\\\ ? +=(){}%&¤#\" <>')"));
        }

        [Test]
        public void DeleteAllRecords()
        {
            Assert.That(factory.DeleteAll("WebsiteOrder"), Is.EqualTo("DELETE FROM [WebsiteOrder]"));
        }

        [Test]
        public void DeleteWhereConditionTrue()
        {
            ICondition condition = Field.Equals("Amount", 120);
            Assert.That(factory.DeleteWhere("WebsiteOrder", condition), Is.EqualTo("DELETE FROM [WebsiteOrder] WHERE ([Amount] = 120)"));
        }

        [Test]
        public void GetCount()
        {
            IScalarExpression expression = Scalar.Count("Entity");
            Assert.That(factory.SelectScalar(expression), Is.EqualTo("SELECT COUNT(*) FROM [Entity]"));
        }

        [Test]
        public void GetAverage()
        {
            IScalarExpression expression = Scalar.Average("Age", "People");
            Assert.That(factory.SelectScalar(expression), Is.EqualTo("SELECT AVG([Age]) FROM [People]"));
        }

        [Test]
        public void GetAverageWithCondition()
        {
            IScalarExpression expression = Scalar.Average("Age", "People");
            expression.Condition = Field.Equals("Gender", "Male");

            Assert.That(factory.SelectScalar(expression), Is.EqualTo("SELECT AVG([Age]) FROM [People] WHERE ([Gender] = 'Male')"));
        }

        [Test]
        public void GetSumWithCondition()
        {
            var expression = Scalar.Sum("Age", "People", Field.Equals("Gender", "Female"));
            Assert.That(factory.SelectScalar(expression), Is.EqualTo("SELECT SUM([Age]) FROM [People] WHERE ([Gender] = 'Female')"));
        }

        [Test]
        public void GetCountWithCondition()
        {
            IScalarExpression expression = Scalar.Count("Entity", Field.Equals("Gender", "Unknown"));
            Assert.That(factory.SelectScalar(expression), Is.EqualTo("SELECT COUNT(*) FROM [Entity] WHERE ([Gender] = 'Unknown')"));
        }
    }
}