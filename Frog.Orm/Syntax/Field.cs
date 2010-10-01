using System;
using System.Collections.Generic;
using System.Linq;
using Frog.Orm.Conditions;

namespace Frog.Orm.Syntax
{
    public class Field
    {
        public static ICondition Equals(string column, long value)
        {
            return new EqualsCondition(column, value);
        }

        public static ICondition Equals(string column, string value)
        {
            return new EqualsCondition(column, value);
        }

        public static ICondition Equals(string column, Guid value)
        {
            return new EqualsCondition(column, value);
        }

        public static ICondition Equals(string column, DateTime value)
        {
            throw new NotImplementedException();
        }

        public static ICondition In(string column, string[] values)
        {
            return new InCondition(column, values);
        }

        public static ICondition In(string column, long[] values)
        {
            return new InCondition(column, values.Select(v => v as object).ToArray());
        }

        public static ICondition Contains(string column, string value)
        {
            return new ContainsCondition(column, value);
        }

        public static ICondition StartsWith(string column, string value)
        {
            return new StartsWithCondition(column, value);
        }

        public static ICondition EndsWith(string column, string value)
        {
            return new EndsWithCondition(column, value);
        }

        public static ICondition GreaterThan(string column, int value)
        {
            return new GreaterThanCondition(column, value);
        }

        public static ICondition LessThan(string column, int value)
        {
            return new LessThanCondition(column, value);
        }

        public static ICondition LessOrEqualTo(string column, int value)
        {
            throw new NotImplementedException();
        }

        public static ICondition GreaterOrEqualTo(string column, int value)
        {
            throw new NotImplementedException();
        }

        public static ICondition EarlierThan(string column, DateTime value)
        {
            throw new NotImplementedException();
        }

        public static ICondition LaterThan(string column, DateTime value)
        {
            throw new NotImplementedException();
        }

        public static ICondition And(ICondition condition1, ICondition condition2)
        {
            return new AndCondition(condition1, condition2);
        }

        public static ICondition Or(ICondition condition1, ICondition condition2)
        {
            return new OrCondition(condition1, condition2);
        }

        public static ICondition Not(ICondition condition)
        {
            return new NotCondition(condition);
        }

        public static FieldList List(params string[] list)
        {
            return new FieldList(list);
        }
    }

    public class FieldList
    {
        public FieldList(params string[] list)
        {
            Fields = list;
        }

        public string[] Fields { get; set; }
    }
}