
using System;
namespace Frog.Orm.Dialects
{
    public class TransactSqlDialect : SqlDialectBase
    {
        protected override string MapValueToSql(object value)
        {
            if (value is String)
            {
                return String.Format("N'{0}'", Escape(value.ToString()));
            }
            return base.MapValueToSql(value);
        }
    }
}