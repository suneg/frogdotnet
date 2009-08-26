using System;
using Frog.Orm.Syntax;

namespace Frog.Orm.Dialects
{
    public class SqliteDialect : SqlDialectBase
    {
        public override string SelectIdentity()
        {
            return "SELECT last_insert_rowid()";
        }

        public override string Select(string tableName, FieldList list)
        {
            var columnList = String.Join(",", list.Fields);
            return String.Format("SELECT {0} FROM [{1}]", columnList, tableName);
        }

        protected override string MapValueToSql(object value)
        {
            if (value is Boolean)
                return (bool)value ? "1" : "0";

            return base.MapValueToSql(value);
        }
    }
}