using System.Collections.Generic;
using Frog.Orm.Conditions;

namespace Frog.Orm.Dialects
{
    public interface ISqlDialect
    {
        string Select(string tableName, params string[] columns);
        string SelectWhere(string tableName, ICondition condition, params string[] columns);
        string Update(string tableName, Dictionary<string, object> columnValueCollection);
        string UpdateWhere(string tableName, ICondition condition, Dictionary<string, object> columnValueCollection);
        string Insert(string tableName, Dictionary<string, object> columnValueCollection);
        string DeleteWhere(string tableName, ICondition condition);
        string DeleteAll(string tableName);
        string SelectIdentity();
    }
}