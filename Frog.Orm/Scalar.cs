using Frog.Orm.Conditions;

namespace Frog.Orm
{
    public class Scalar
    {
        public static IScalarExpression Count(string sourceName)
        {
            return new ScalarCountExpression(sourceName);
        }

        public static IScalarExpression Average(string fieldName, string sourceName)
        {
            return new ScalarAverageExpression(fieldName, sourceName, null);
        }

        public static IScalarExpression Average(string fieldName, string sourceName, ICondition condition)
        {
            return new ScalarAverageExpression(fieldName, sourceName, condition);
        }
    }

    public abstract class BaseScalarExpression : IScalarExpression
    {
        public string SourceName { get; set; }
        public string FieldName { get; set; }
        public ICondition Condition { get; set; }

        public bool HasCondition
        {
            get
            {
                return Condition != null;
            }
        }
    }

    public class ScalarAverageExpression : BaseScalarExpression
    {
        public ScalarAverageExpression(string fieldName, string sourceName, ICondition condition)
        {
            FieldName = fieldName;
            SourceName = sourceName;
            Condition = condition;
        }
    }

    public class ScalarCountExpression : BaseScalarExpression
    {
        public ScalarCountExpression(string sourceName)
        {
            SourceName = sourceName;
            FieldName = "*";
        }
    }
}

