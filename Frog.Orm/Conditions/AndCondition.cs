namespace Frog.Orm.Conditions
{
    public class AndCondition : ICondition
    {
        private readonly ICondition condition1;
        private readonly ICondition condition2;

        public AndCondition(ICondition condition1, ICondition condition2)
        {
            this.condition1 = condition1;
            this.condition2 = condition2;
        }

        public ICondition Condition2
        {
            get { return condition2; }
        }

        public ICondition Condition1
        {
            get { return condition1; }
        }
    }
}