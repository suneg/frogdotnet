using System.Collections.Generic;

namespace Frog.Orm.Conditions
{
    public class InCondition : ColumnCondition
    {
        public InCondition(string column, object[] values) : base(column, values)
        {
            
        }
    }
}