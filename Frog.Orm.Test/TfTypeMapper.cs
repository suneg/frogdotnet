using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace Frog.Orm.Test
{
    [TestFixture]
    public class TfTypeMapper
    {
        private TypeMapper mapper;

        [SetUp]
        public void Setup()
        {
            mapper = new TypeMapper();
        }

        [Test]
        public void MapEmptyType()
        {
            var info = mapper.GetTypeInfo(typeof (Empty));
            Assert.That(info.TableName, Is.EqualTo("Empty"));
        }

        [Test]
        public void MapOverriddenTableName()
        {
            var info = mapper.GetTypeInfo(typeof (Whack));
            Assert.That(info.TableName, Is.EqualTo("Whacky_Table_Name"));
        }

        [Test]
        public void MapPrimaryKey()
        {
            var info = mapper.GetTypeInfo(typeof (WithPrimaryKey));
            Assert.That(info.PrimaryKey, Is.EqualTo("PrimaryColumn"));
        }

        [Test]
        public void MapOverriddenPrimaryKey()
        {
            var info = mapper.GetTypeInfo(typeof(WithOverriddenPrimaryKey));
            Assert.That(info.PrimaryKey, Is.EqualTo("Id"));
        }

        [Test]
        public void MapTypeWithColumns()
        {
            var info = mapper.GetTypeInfo(typeof (WithColumns));
            Assert.That(info.Columns.Count, Is.EqualTo(2));

            var first = info.Columns[0];
            Assert.That(first.Name, Is.EqualTo("ColumnX"));
            Assert.That(first.Type, Is.EqualTo(typeof(Int32)));

            var second = info.Columns[1];
            Assert.That(second.Name, Is.EqualTo("ColumnY"));
            Assert.That(second.Type, Is.EqualTo(typeof(String)));
        }

        [Test]
        public void MapOverriddenColumnName()
        {
            var info = mapper.GetTypeInfo(typeof(WithOverridenColumnName));
            Assert.That(info.Columns[0].Name, Is.EqualTo("SimpleName"));
        }

        [Test]
        public void VerifyPrimaryKeyPresentInListOfColumns()
        {
            var info = mapper.GetTypeInfo(typeof(WithOverriddenPrimaryKey));

            Assert.That(info.Columns.Count, Is.EqualTo(1));
            Assert.That(info.Columns[0].Name, Is.EqualTo("Id"));
        }

        [Test]
        public void MapInstanceToDictionary()
        {
            var instance = new WithColumns();
            instance.ColumnX = 5;
            instance.ColumnY = "test";

            var instanceValues = mapper.GetInstanceValues(instance);

            Assert.That(instanceValues.Count, Is.EqualTo(2));
            Assert.That(instanceValues["ColumnX"], Is.EqualTo(5));
            Assert.That(instanceValues["ColumnY"], Is.EqualTo("test"));
        }


        [Test]
        public void MapInstanceWithOverriddenPrimaryKeyToValuesDictionary()
        {
            var instance = new WithOverriddenPrimaryKey();
            instance.PrimaryColumn = 18;

            var instanceValues = mapper.GetInstanceValues(instance);

            Assert.That(instanceValues.Count, Is.EqualTo(1));
            Assert.That(instanceValues["Id"], Is.EqualTo(18));
        }

        [Test]
        public void GetValueOfPrimaryKey()
        {
            var instance = new WithOverriddenPrimaryKey();
            instance.PrimaryColumn = 18;

            Assert.That(mapper.GetValueOfPrimaryKey(instance), Is.EqualTo(18));
        }

        [Test]
        public void GetValueOfGuidBasedPrimaryKey()
        {
            var theGuid = Guid.NewGuid();
            var instance = new WithGuidBasedPrimaryKey();

            instance.PrimaryKey = theGuid;

            Assert.That(mapper.GetValueOfPrimaryKey(instance), Is.EqualTo(theGuid));    
        }

        [Test]
        public void GetValueOfPrimaryKeyOnTypeWithNoPrimaryKey()
        {
            var instance = new WithColumns();
            
            Assert.That(mapper.GetValueOfPrimaryKey(instance), Is.Null);
        }

        [Test, ExpectedException(typeof(MappingException))]
        public void MapTypeWithMissingTableAttribute()
        {
            mapper.GetTypeInfo(typeof(TypeWithNoAttributes));
        }
    }

    #region Sample Mapping classes

    [Table]
    internal class Empty
    {
        
    }

    internal class TypeWithNoAttributes
    {
    }

    [Table(Name = "Whacky_Table_Name")]
    internal class Whack
    {
        
    }

    [Table]
    internal class WithPrimaryKey
    {
        [PrimaryKey]
        public int PrimaryColumn { get; set; }
    }

    [Table]
    internal class WithOverriddenPrimaryKey
    {
        [PrimaryKey(Name = "Id")]
        public int PrimaryColumn { get; set; }
    }

    [Table]
    internal class WithGuidBasedPrimaryKey
    {
        [PrimaryKey]
        public Guid PrimaryKey { get; set; }
    }

    [Table]
    internal class WithColumns
    {
        [Column]
        public int ColumnX { get; set; }

        [Column]
        public string ColumnY { get; set; }
    }

    [Table]
    internal class WithOverridenColumnName
    {
        [Column(Name = "SimpleName")]
        public double ComplexColumnName { get; set; }
    }

    #endregion
}
