using System.Linq;
using Frog.Orm.Syntax;
using NUnit.Framework;

namespace Frog.Orm.Test
{
    public abstract class DatabaseWriteTests
    {
        protected IConnection connection;

        [Test]
        public void StoreNewInstance()
        {
            var entity = new Entity {Text = "hello world"};

            var repository = new Repository(connection);
            var returnedObject = repository.Create(entity);
            connection.CommitChanges();

            var storedEntity = repository.Get<Entity>(returnedObject.Id);
            Assert.That(storedEntity.Text, Is.EqualTo("hello world"));
        }

        [Test]
        public void StoreFast()
        {
            var repository = new Repository(connection);

            var count = repository.GetAll<Entity>().Count();
            Assert.AreEqual(count, 0);

            var entity = new Entity { Text = "hello world" };
            repository.CreateFast(entity);
            connection.CommitChanges();

            count = repository.GetAll<Entity>().Count();
            Assert.AreEqual(count, 1);
        }

        [Test]
        public void UpdateInstance()
        {
            var entity = new Entity();

            var repository = new Repository(connection);
            entity = repository.Create(entity);
            connection.CommitChanges();

            entity.Text = "hello everyone";
            repository.Update(entity);

            var updatedObject = repository.Get<Entity>(entity.Id);
            Assert.That(updatedObject.Text, Is.EqualTo("hello everyone"));
        }

        [Test]
        public void SupportUnicode()
        {
            var entity = new Entity { Text = "hello ٩(͡๏̯͡๏)۶ ٩(-̮̮̃•̃)." };
            var repository = new Repository(connection);
            var returnedObject = repository.Create(entity);
            connection.CommitChanges();

            var storedEntity = repository.Get<Entity>(returnedObject.Id);
            Assert.That(storedEntity.Text, Is.EqualTo("hello ٩(͡๏̯͡๏)۶ ٩(-̮̮̃•̃)."));

            returnedObject.Text = "hey ٩(͡๏̯͡๏)۶ ٩(-̮̮̃•̃).";
            repository.Update(returnedObject);

            var updatedObject = repository.Get<Entity>(returnedObject.Id);
            Assert.That(updatedObject.Text, Is.EqualTo("hey ٩(͡๏̯͡๏)۶ ٩(-̮̮̃•̃)."));

            var foundObject = repository.GetWhere<Entity>(Field.Equals("Text", "hey ٩(͡๏̯͡๏)۶ ٩(-̮̮̃•̃).")).Single();
            Assert.That(foundObject.Id, Is.EqualTo(updatedObject.Id));
        }

        [Test]
        public void RemoveInstance()
        {
            var entity = new Entity();

            var repository = new Repository(connection);
            entity = repository.Create(entity);
            
            Assert.That(repository.GetAll<Entity>().Count(), Is.EqualTo(1));

            repository.Remove(entity);
            Assert.That(repository.GetAll<Entity>().Count(), Is.EqualTo(0));
        }

        [Test]
        public void RemoveMultiple()
        {
            var repository = new Repository(connection);
            CreateThreeEntities(repository);

            Assert.That(repository.GetAll<Entity>().Count(), Is.EqualTo(3));

            repository.RemoveWhere<Entity>(Field.StartsWith("Text", "test"));
            Assert.That(repository.GetAll<Entity>().Count(), Is.EqualTo(1));
        }

        [Test]
        public void RemoveAll()
        {
            var repository = new Repository(connection);
            CreateThreeEntities(repository);

            Assert.That(repository.GetAll<Entity>().Count(), Is.EqualTo(3));

            repository.RemoveAll<Entity>();
            Assert.That(repository.GetAll<Entity>().Count(), Is.EqualTo(0));
        }

        private static void CreateThreeEntities(Repository repository)
        {
            var entity1 = new Entity { Text = "testing" };
            var entity2 = new Entity { Text = "testing" };
            var entity3 = new Entity { Text = "something else" };
                
            repository.Create(entity1);
            repository.Create(entity2);
            repository.Create(entity3);
        }

        [Test]
        public void StoreTypeWithEnum()
        {
            var repository = new Repository(connection);
            var entity = new TypeWithEnumMember();
            entity.ActualEnumValue = SampleEnum.B;

            repository.Create(entity);
            var entities = repository.GetAll<TypeWithEnumMember>();
            entities = entities.ToList();

            Assert.That(entities.Count(), Is.EqualTo(1));
            Assert.That(entities.First().ActualEnumValue, Is.EqualTo(SampleEnum.B));
        }

        [Test]
        public void StoreTypeWithBoolean()
        {
            var repository = new Repository(connection);
            var entity = new TypeWithBoolean();
            entity.FirstBooleanValue = true;
            entity.LastBooleanValue = false;

            repository.Create(entity);
            var entities = repository.GetAll<TypeWithBoolean>();
            entities = entities.ToList();

            Assert.That(entities.Count(), Is.EqualTo(1));

            var storedEntity = entities.First();
            Assert.That(storedEntity.FirstBooleanValue, Is.True);
            Assert.That(storedEntity.LastBooleanValue, Is.False);
        }

        [Test]
        public void AttemptSqlInjectionWithApostropheDuringUpdate()
        {
            var repository = new Repository(connection);
            var entity = new Entity();
            entity = repository.Create(entity);

            entity.Text = "hello' everyone";

            repository.Update(entity);
            connection.CommitChanges();

            var updatedObject = repository.Get<Entity>(entity.Id);
            Assert.That(updatedObject.Text, Is.EqualTo("hello' everyone"));
        
        }

        [Test]
        public void AttemptSqlInjectionWithApostropheDuringInsert()
        {
            var repository = new Repository(connection);
            var entity = new Entity();
            entity.Text = "hello' everyone";
            entity = repository.Create(entity);

            connection.CommitChanges();

            var updatedObject = repository.Get<Entity>(entity.Id);
            Assert.That(updatedObject.Text, Is.EqualTo("hello' everyone"));
        }
    }

    [Table]
    internal class Entity
    {
        [PrimaryKey]
        public long Id { get; set; }

        [Column]
        public string Text { get; set; }

        //[Column]
        public string Text2 { get; set; }

        //[Column]
        public string Text3 { get; set; }
    }

    [Table]
    internal class TypeWithBoolean
    {
        [PrimaryKey]
        public long Id { get; set; }

        [Column]
        public bool FirstBooleanValue { get; set; }

        [Column]
        public bool LastBooleanValue { get; set; }
    }
}