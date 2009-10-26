using System;
using System.Collections.Generic;
using Frog.Orm.Conditions;

namespace Frog.Orm
{
    public class Repository : BaseRepository, IRepository
    {
        public Repository(IConnection connection) : base(connection)
        {
            connection.DataEnumerator = new DataEnumerator(this);            
        }

        public new T Get<T>(long primaryKeyValue)
        {
            return base.Get<T>(primaryKeyValue);
        }

        public new T Get<T>(Guid primaryKeyValue)
        {
            return base.Get<T>(primaryKeyValue);
        }

        public new IEnumerable<T> GetAll<T>()
        {
            return base.GetAll<T>();
        }

        public new IEnumerable<T> GetWhere<T>(ICondition condition)
        {
            return base.GetWhere<T>(condition);
        }

        public new T Create<T>(T obj)
        {
            return base.Create(obj);
        }

        public void CreateFast<T>(T obj)
        {
            base.CreateFast(obj);
        }

        public new void Remove(object obj)
        {
            base.Remove(obj);
        }

        public new void RemoveAll<T>()
        {
            base.RemoveAll<T>();
        }

        public new void RemoveWhere<T>(ICondition condition)
        {
            base.RemoveWhere<T>(condition);
        }

        public new void Update(object obj)
        {
            base.Update(obj);
        }
    }
}
