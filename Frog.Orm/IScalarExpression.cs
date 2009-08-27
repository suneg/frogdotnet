using Frog.Orm.Conditions;

namespace Frog.Orm
{
    public interface IScalarExpression
    {
        string SourceName { get; }
        string FieldName { get; }
        ICondition Condition { get; set; }
        bool HasCondition { get; }
    }
}