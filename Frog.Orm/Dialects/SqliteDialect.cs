using System;

namespace Frog.Orm.Dialects
{
    public class SqliteDialect : SqlDialectBase
    {
        public override string SelectIdentity()
        {
            return "SELECT last_insert_rowid()";
        }

        public override string Select(string tableName, params string[] columns)
        {
            var columnList = String.Join(",", columns);
            return String.Format("SELECT {0} FROM [{1}]", columnList, tableName);
        }
    }
}