namespace Frog.Orm.Conditions
{
    public class EndsWithCondition : ColumnCondition
    {
        public EndsWithCondition(string column, string value)
            : base(column, value)
        {

        }
    }
}