using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace Frog.Orm.Test
{
    [TestFixture]
    public class TfTypeCache
    {
        private TypeCache typeCache;

        [SetUp]
        public void Setup()
        {
            typeCache = new TypeCache();
        }

        [Test]
        public void AddTypeWithPrimaryKeyToCache()
        {    
            var type = typeof(WithPrimaryKey);
            Assert.That(typeCache.GetTypeInfo(type).PrimaryKey.Name, Is.EqualTo("PrimaryColumn"));
        }

        [Test]
        public void AddTypeWithOverridenPrimaryKeyToCache()
        {
            var type = typeof(WithOverriddenPrimaryKey);
            Assert.That(typeCache.GetTypeInfo(type).PrimaryKey.Name, Is.EqualTo("Id"));
        }

        [Test]
        public void AddTypeWithColumns()
        {
            var type = typeof (WithColumns);
            
            SecondMappedTypeInfo info = typeCache.GetTypeInfo(type);
            Assert.That(info.Columns.Count, Is.EqualTo(2));
            Assert.That(info.Columns[0].Name, Is.EqualTo("ColumnX"));
            Assert.That(info.Columns[1].Name, Is.EqualTo("ColumnY"));
            
        }
    }
}
