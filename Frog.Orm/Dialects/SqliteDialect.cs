using System;
using Frog.Orm.Conditions;
using Frog.Orm.Syntax;

namespace Frog.Orm.Dialects
{
    public class SqliteDialect : SqlDialectBase
    {
        public override string SelectIdentity()
        {
            return "SELECT last_insert_rowid()";
        }

        public override string SelectWhere(string tableName, FieldList list, ICondition condition, Order order)
        {
            var columnList = String.Join(",", list.Fields);
            var statement = String.Format("SELECT {0} FROM [{1}]", columnList, tableName);

            if (condition != null)
                statement += String.Format(" WHERE {0}", GetWhereClause(condition));

            if (order != null)
                statement += String.Format(" ORDER BY {0}", GetOrderClause(order));

            return statement;
        }

        protected override string MapValueToSql(object value)
        {
            if (value is Boolean)
                return (bool)value ? "1" : "0";

            return base.MapValueToSql(value);
        }
    }
}