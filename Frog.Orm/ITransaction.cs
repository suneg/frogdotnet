using System;
using System.Collections.Generic;
using Frog.Orm.Conditions;

namespace Frog.Orm
{
    public interface ITransaction
    {
        T GetByPrimaryKey<T>(long primaryKeyValue);
        T GetByPrimaryKey<T>(Guid primaryKeyValue);
        T GetSingle<T>(string sourceName, ICondition condition);
        IEnumerable<T> GetAll<T>();
        IEnumerable<T> GetMany<T>(string sourceName, ICondition condition);
        IEnumerable<T> GetWhere<T>(ICondition condition);
        IEnumerable<T> ExecuteRaw<T>(string sqlStatement);
        
        void Rollback();
        void Commit();

        T Create<T>(T obj);
        void Update(object instance);
        void Delete(object instance);
        void DeleteWhere<T>(ICondition condition);
        void DeleteAll<T>();
    }
}
