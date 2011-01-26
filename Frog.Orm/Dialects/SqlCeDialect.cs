using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Frog.Orm.Dialects
{
    class SqlCeDialect : SqlDialectBase
    {
        public override string SelectIdentity()
        {
            return "SELECT @@identity";
        }
    }
}
