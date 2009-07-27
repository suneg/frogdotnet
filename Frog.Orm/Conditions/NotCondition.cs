namespace Frog.Orm.Conditions
{
    public class NotCondition : ICondition
    {
        private readonly ICondition condition;

        public NotCondition(ICondition condition)
        {
            this.condition = condition;
        }

        public ICondition InverseCondition
        {
            get { return condition; }
        }
    }
}
