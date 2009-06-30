namespace Frog.Orm.Conditions
{
    public class ContainsCondition : ColumnCondition
    {
        public ContainsCondition(string column, string value) : base(column, value)
        {
            
        }
    }
}