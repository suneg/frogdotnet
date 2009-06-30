namespace Frog.Orm.Conditions
{
    public class StartsWithCondition : ColumnCondition
    {
        public StartsWithCondition(string column, string value)
            : base(column, value)
        {

        }
    }
}