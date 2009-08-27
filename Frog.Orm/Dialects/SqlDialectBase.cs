using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Frog.Orm.Conditions;
using Frog.Orm.Syntax;

namespace Frog.Orm.Dialects
{
    public abstract class SqlDialectBase : ISqlDialect
    {
        public virtual string Select(string tableName, FieldList list)
        {
            return SelectWhere(tableName, list, null, null);
        }

        public virtual string SelectWhere(string tableName, FieldList list, Order order)
        {
            return SelectWhere(tableName, list, null, order);
        }

        public virtual string SelectWhere(string tableName, FieldList list, ICondition condition)
        {
            return SelectWhere(tableName, list, condition, null);
        }

        public virtual string SelectWhere(string tableName, FieldList list, ICondition condition, Order order)
        {
            var columnList = String.Join("],[", list.Fields);
            var statement = String.Format("SELECT [{0}] FROM [{1}]", columnList, tableName);

            if(condition != null)
                statement += String.Format(" WHERE {0}", GetWhereClause(condition));

            if(order != null)
                statement += String.Format(" ORDER BY {0}", GetOrderClause(order));
            
            return statement;
        }

        public virtual string Update(string tableName, Dictionary<string, object> columnValueCollection)
        {
            var columnUpdates = new StringBuilder();

            for (var i = 0; i < columnValueCollection.Keys.Count; i++)
            {
                var key = columnValueCollection.Keys.ElementAt(i);
                var value = columnValueCollection[key];

                columnUpdates.AppendFormat("[{0}] = ", key);

                columnUpdates.Append(MapValueToSql(value));

                if (i < columnValueCollection.Keys.Count - 1)
                    columnUpdates.Append(", ");
            }

            return String.Format("UPDATE [{0}] SET {1}", tableName, columnUpdates);
        }

        public virtual string UpdateWhere(string tableName, ICondition condition, Dictionary<string, object> columnValueCollection)
        {
            var updateStatement = Update(tableName, columnValueCollection);
            return String.Format("{0} WHERE {1}", updateStatement, GetWhereClause(condition));
        }

        public virtual string Insert(string tableName, Dictionary<string, object> columnValueCollection)
        {
            if(columnValueCollection.Count == 0)
                return String.Format("INSERT INTO [{0}] DEFAULT VALUES", tableName);

            var columns = String.Join("],[", columnValueCollection.Keys.ToArray());

            var values = new StringBuilder();

            for (var i = 0; i < columnValueCollection.Keys.Count; i++)
            {
                var key = columnValueCollection.Keys.ElementAt(i);
                var value = columnValueCollection[key];

                values.Append(MapValueToSql(value));

                if (i < columnValueCollection.Keys.Count - 1)
                    values.Append(",");
            }

            return String.Format("INSERT INTO [{0}]([{1}]) VALUES({2})", tableName, columns, values);
        }

        protected virtual string MapValueToSql(object value)
        {
            if (value is Int32)
            {
                return String.Format("{0}", value);
            }
            if (value is Int64)
            {
                return String.Format("{0}", value);
            }
            if (value is Decimal)
            {
                return String.Format("{0}", value);
            }
            if (value is Boolean)
            {
                return String.Format("'{0}'", value);        // TODO: SqlServer supported??! true vs. True vs. 1 vs. 'true' ??
            }
            if (value is String)
            {
                return String.Format("'{0}'", Escape(value.ToString()));
            }
            if (value is DateTime)
            {
                var dateTime = (DateTime)value;
                return String.Format("'{0}'", dateTime.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            }
            if (value is Guid)
            {
                return String.Format("'{0}'", value);
            }
            if (value == null)
            {
                return String.Format("NULL");
            }
            if( value.GetType().IsEnum)
            {
                return String.Format("{0}", Convert.ToInt32(value));
            }
            
            throw new InvalidOperationException(String.Format("Attempted to update an unsupported datatype ({0})", value.GetType().FullName));
            
        }

        protected virtual string GetWhereClause(ICondition condition)
        {
            if(condition is EqualsCondition)
            {
                var clause = (condition as EqualsCondition);
                return String.Format("([{0}] = {1})", clause.Column, MapValueToSql(clause.Value));
            }

            if(condition is StartsWithCondition)
            {
                var clause = (condition as StartsWithCondition);
                return String.Format("([{0}] LIKE '{1}%')", clause.Column, Escape(clause.Value.ToString()));
            }

            if(condition is EndsWithCondition)
            {
                var clause = (condition as EndsWithCondition);
                return String.Format("([{0}] LIKE '%{1}')", clause.Column, Escape(clause.Value.ToString()));
            }

            if (condition is ContainsCondition)
            {
                var clause = (condition as ContainsCondition);
                return String.Format("([{0}] LIKE '%{1}%')", clause.Column, Escape(clause.Value.ToString()));
            }

            if(condition is AndCondition)
            {
                var clause = (condition as AndCondition);
                var condition1 = GetWhereClause(clause.Condition1);
                var condition2 = GetWhereClause(clause.Condition2);

                return String.Format("({0} AND {1})", condition1, condition2);
            }

            if(condition is OrCondition)
            {
                var clause = (condition as OrCondition);
                var condition1 = GetWhereClause(clause.Condition1);
                var condition2 = GetWhereClause(clause.Condition2);

                return String.Format("({0} OR {1})", condition1, condition2);
            }

            if (condition is GreaterThanCondition)
            {
                var clause = (condition as GreaterThanCondition);
                return String.Format("([{0}] > {1})", clause.Column, MapValueToSql(clause.Value));
            }

            if (condition is LessThanCondition)
            {
                var clause = (condition as LessThanCondition);
                return String.Format("([{0}] < {1})", clause.Column, MapValueToSql(clause.Value));
            }

            if (condition is NotCondition)
            {
                var clause = (condition as NotCondition);
                return String.Format("(NOT {0})", GetWhereClause(clause.InverseCondition));
            }

            throw new InvalidOperationException(String.Format("Unsupported Condition ({0})", condition.GetType().FullName));
        }

        private static string GetOrderClause(Order order)
        {
            var builder = new StringBuilder();

            foreach(var orderPart in order.Parts)
            {
                builder.AppendFormat("[{0}] ", orderPart.Column);

                if(orderPart.Direction == OrderDirection.Ascending)
                    builder.Append("ASC,");
                else
                    builder.Append("DESC,");
            }

            return builder.ToString().TrimEnd(',');
        }

        protected static string Escape(string value)
        {
            return value.Replace("'", "''");
        }

        public virtual string DeleteWhere(string tableName, ICondition condition)
        {
            return String.Format("DELETE FROM [{0}] WHERE {1}", tableName, GetWhereClause(condition));
        }

        public virtual string DeleteAll(string tableName)
        {
            return String.Format("DELETE FROM [{0}]", tableName);
        }

        public virtual string SelectIdentity()
        {
            return "SELECT @@IDENTITY";
        }

        public string SelectScalar(IScalarExpression expression)
        {
            string result = null;
            string fieldName = expression.FieldName;

            if (fieldName != "*")
                fieldName = String.Format("[{0}]", fieldName);

            if(expression is ScalarCountExpression)
            {
                result = String.Format("SELECT COUNT({0}) FROM [{1}]", fieldName, expression.SourceName);
            }
            if(expression is ScalarAverageExpression)
            {
                result = String.Format("SELECT AVG({0}) FROM [{1}]", fieldName, expression.SourceName);
            }

            if (expression.HasCondition)
                result += String.Format(" WHERE {0}", GetWhereClause(expression.Condition));

            if(result != null)
                return result;

            throw new InvalidOperationException(String.Format("Unsupported scalar expression ({0})", expression.GetType().FullName));
        }
    }
}