using System;
using System.Collections.Generic;
using Frog.Orm.Conditions;

namespace Frog.Orm
{
    public interface IRepository : IDisposable
    {
        T Get<T>(long primaryKeyValue);
        T Get<T>(Guid primaryKeyValue);
        IEnumerable<T> GetAll<T>();
        IEnumerable<T> GetWhere<T>(ICondition condition);
        void CommitChanges();
        T Create<T>(T obj);
        void Remove(object obj);
        void Update(object obj);
    }
}