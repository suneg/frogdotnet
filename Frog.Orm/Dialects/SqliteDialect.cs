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
    }
}