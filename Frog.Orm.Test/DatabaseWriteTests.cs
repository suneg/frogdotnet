using System.Linq;
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

            using (var repository = new Repository(connection))
            {
                var returnedObject = repository.Create(entity);
                repository.CommitChanges();

                var storedEntity = repository.Get<Entity>(returnedObject.Id);
                Assert.That(storedEntity.Text, Is.EqualTo("hello world"));
            }
        }

        [Test]
        public void UseMultipleRepositoriesWithSingleConnection()
        {
            var entity1 = new Entity {Text = "Hello World" };
            var entity2 = new Entity {Text = "Hello World" };

            var repository1 = new Repository(connection);
            var repository2 = new Repository(connection);


            repository1.Create(entity1);
            repository1.Create(entity2);

            repository1.CommitChanges();
            repository2.CommitChanges();
        }

        [Test]
        public void UpdateInstance()
        {
            var entity = new Entity();

            using (var repository = new Repository(connection))
            {
                entity = repository.Create(entity);
                repository.CommitChanges();

                entity.Text = "hello everyone";

                repository.Update(entity);
                repository.CommitChanges();

                var updatedObject = repository.Get<Entity>(entity.Id);
                Assert.That(updatedObject.Text, Is.EqualTo("hello everyone"));
            }
        }

        [Test]
        public void RemoveInstance()
        {
            var entity = new Entity();

            using (var repository = new Repository(connection))
            {
                entity = repository.Create(entity);
                repository.CommitChanges();
                Assert.That(repository.GetAll<Entity>().Count(), Is.EqualTo(1));

                repository.Remove(entity);
                Assert.That(repository.GetAll<Entity>().Count(), Is.EqualTo(0));
            }
        }

        [Test]
        public void StoreTypeWithEnum()
        {
            using (var repository = new Repository(connection))
            {
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
            using (var repository = new Repository(connection))
            {
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
            
            using (var repository = new Repository(connection))
            {
                var entity = new Entity();
                entity = repository.Create(entity);

                entity.Text = "hello' everyone";

                repository.Update(entity);
                repository.CommitChanges();

                var updatedObject = repository.Get<Entity>(entity.Id);
                Assert.That(updatedObject.Text, Is.EqualTo("hello' everyone"));
            }
        }

        [Test]
        public void AttemptSqlInjectionWithApostropheDuringInsert()
        {

            using (var repository = new Repository(connection))
            {
                var entity = new Entity();
                entity.Text = "hello' everyone";
                entity = repository.Create(entity);

                repository.CommitChanges();

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