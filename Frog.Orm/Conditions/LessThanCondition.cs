namespace Frog.Orm.Conditions
{
    public class LessThanCondition : ColumnCondition
    {
        public LessThanCondition(string column, object value)
            : base(column, value)
        {

        }
    }
}