namespace Frog.Orm.Conditions
{
    public class GreaterThanCondition : ColumnCondition
    {
        public GreaterThanCondition(string column, object value)
            : base(column, value)
        {

        }
    }
}