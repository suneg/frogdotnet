namespace Frog.Orm.Conditions
{
    public abstract class ColumnCondition : ICondition
    {
        private readonly string column;
        private readonly object value;

        protected ColumnCondition(string column, object value)
        {
            this.column = column;
            this.value = value;
        }

        public object Value
        {
            get { return value; }
        }

        public string Column
        {
            get { return column; }
        }
    }
}
