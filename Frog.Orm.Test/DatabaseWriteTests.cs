using System.Linq;
using Frog.Orm.Syntax;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace Frog.Orm.Test
{
    public abstract class DatabaseWriteTests
    {
        protected IConnection connection;

        [Test]
        public void StoreNewInstance()
        {
            var entity = new Entity {Text = "hello world"};

            using (connection)
            {
                var repository = new Repository(connection);
                var returnedObject = repository.Create(entity);
                connection.CommitChanges();

                var storedEntity = repository.Get<Entity>(returnedObject.Id);
                Assert.That(storedEntity.Text, Is.EqualTo("hello world"));
            }
        }

        [Test]
        public void UpdateInstance()
        {
            var entity = new Entity();

            using (connection)
            {
                var repository = new Repository(connection);
                entity = repository.Create(entity);
                connection.CommitChanges();

                entity.Text = "hello everyone";

                repository.Update(entity);
                connection.CommitChanges();

                var updatedObject = repository.Get<Entity>(entity.Id);
                Assert.That(updatedObject.Text, Is.EqualTo("hello everyone"));
            }
        }

        [Test]
        public void RemoveInstance()
        {
            var entity = new Entity();

            using (connection)
            {
                var repository = new Repository(connection);
                entity = repository.Create(entity);
                connection.CommitChanges();
                Assert.That(repository.GetAll<Entity>().Count(), Is.EqualTo(1));

                repository.Remove(entity);
                Assert.That(repository.GetAll<Entity>().Count(), Is.EqualTo(0));
            }
        }

        [Test]
        public void RemoveMultiple()
        {
            var entity1 = new Entity { Text = "testing"};
            var entity2 = new Entity { Text = "testing" };
            var entity3 = new Entity { Text = "something else" };

            using (connection)
            {
                var repository = new Repository(connection);
                repository.Create(entity1);
                repository.Create(entity2);
                repository.Create(entity3);

                connection.CommitChanges();
                Assert.That(repository.GetAll<Entity>().Count(), Is.EqualTo(3));

                repository.RemoveWhere<Entity>(Field.StartsWith("Text", "test"));
                Assert.That(repository.GetAll<Entity>().Count(), Is.EqualTo(1));
            }
        }

        [Test]
        public void StoreTypeWithEnum()
        {
            using (connection)
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
        }

        [Test]
        public void StoreTypeWithBoolean()
        {
            using (connection)
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
        }

        [Test]
        public void AttemptSqlInjectionWithApostropheDuringUpdate()
        {
            
            using (connection)
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
        }

        [Test]
        public void AttemptSqlInjectionWithApostropheDuringInsert()
        {

            using (connection)
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
    }

    [Table]
    internal class Entity
    {
        [PrimaryKey]
        public long Id { get; set; }

        [Column]
        public string Text { get; set; }
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