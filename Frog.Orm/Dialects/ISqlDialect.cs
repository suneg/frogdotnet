using System.Collections.Generic;
using Frog.Orm.Conditions;
using Frog.Orm.Syntax;

namespace Frog.Orm.Dialects
{
    public interface ISqlDialect
    {
        string Select(string tableName, FieldList list);
        string SelectWhere(string tableName, FieldList list, ICondition condition);
        string Update(string tableName, Dictionary<string, object> columnValueCollection);
        string UpdateWhere(string tableName, ICondition condition, Dictionary<string, object> columnValueCollection);
        string Insert(string tableName, Dictionary<string, object> columnValueCollection);
        string DeleteWhere(string tableName, ICondition condition);
        string DeleteAll(string tableName);
        string SelectIdentity();
        string SelectScalar(IScalarExpression expression);
    }
}