using System;
using System.Collections.Generic;
using Frog.Orm.Conditions;

namespace Frog.Orm
{
    public interface IRepository
    {
        T Get<T>(long primaryKeyValue);
        T Get<T>(Guid primaryKeyValue);
        IEnumerable<T> GetAll<T>();
        IEnumerable<T> GetWhere<T>(ICondition condition);
        T Create<T>(T obj);
        void Remove(object obj);
        void Update(object obj);
    }
}