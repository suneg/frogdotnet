namespace Frog.Orm.Conditions
{
    public class EqualsCondition : ColumnCondition
    {
        public EqualsCondition(string column, object value)
            : base(column, value)
        {

        }
    }
}