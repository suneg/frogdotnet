using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Frog.Orm.Conditions;
using Frog.Orm.Dialects;
using Frog.Orm.Syntax;
using log4net;

namespace Frog.Orm
{
    public class Transaction : ITransaction
    {
        private readonly IDbTransaction transaction;
        private readonly ISqlDialect dialect;
        private readonly IDataEnumerator dataEnumerator;
        private readonly ILog log;

        public Transaction(IDbTransaction transaction, ISqlDialect dialect, IDataEnumerator dataEnumerator)
        {
            this.transaction = transaction;
            this.dialect = dialect;
            this.dataEnumerator = dataEnumerator;
            log = LogManager.GetLogger(typeof(Transaction));
        }

        public T GetByPrimaryKey<T>(long primaryKeyValue)
        {
            return InternalGetByPrimaryKey<T>(primaryKeyValue);
        }

        public T GetByPrimaryKey<T>(Guid primaryKeyValue)
        {
            return InternalGetByPrimaryKey<T>(primaryKeyValue);
        }

        private T InternalGetByPrimaryKey<T>(object primaryKeyValue)
        {
            var typeInfo = GetTypeInfo(typeof(T));
            var columns = typeInfo.Columns.Select(c => c.Name);

            ICondition condition = CreatePrimaryKeyCondition(typeInfo, primaryKeyValue);

            var commandText = dialect.SelectWhere(typeInfo.TableName, Field.List(columns.ToArray()), condition);
            var command = CreateCommand(commandText);

            // TODO: Catch InvalidOperationException : Sequence contains no elements, and give a better explanation
            return dataEnumerator.GetEnumerator<T>(command.ExecuteReader()).First();
        }

        private static ICondition CreatePrimaryKeyCondition(MappedTypeInfo typeInfo, object primaryKeyValue)
        {
            ICondition condition;
            
            if(primaryKeyValue is Int32)
                condition = Field.Equals(typeInfo.PrimaryKey, (int)primaryKeyValue);
            else if (primaryKeyValue is Int64)
                condition = Field.Equals(typeInfo.PrimaryKey, (long)primaryKeyValue);
            else if (primaryKeyValue is Guid)
                condition = Field.Equals(typeInfo.PrimaryKey, (Guid)primaryKeyValue);
            else
                throw new NotSupportedException(
                    String.Format("Object of type {0} cannot be used as primary key", primaryKeyValue.GetType().FullName));

            return condition;
        }

        public IEnumerable<T> GetAll<T>()
        {
            return GetWhere<T>(null);
        }

        public IEnumerable<T> GetMany<T>(string sourceName, ICondition condition)
        {
            string commandText = SelectFrom<T>(sourceName, condition);

            var command = CreateCommand(commandText);
            return dataEnumerator.GetEnumerator<T>(CreateLateBoundReader(command)); 
        }

        public T GetSingle<T>(string sourceName, ICondition condition)
        {
            var commandText = SelectFrom<T>(sourceName, condition);

            var command = CreateCommand(commandText);
            return dataEnumerator.GetEnumerator<T>(command.ExecuteReader()).First();
        }

        private string SelectFrom<T>(string sourceName, ICondition condition)
        {
            var typeInfo = GetTypeInfo(typeof(T));
            var columns = typeInfo.Columns.Select(c => c.Name).ToArray();

            string commandText;

            if (condition == null)
            {
                commandText = dialect.Select(sourceName, Field.List(columns));
            }
            else
            {
                commandText = dialect.SelectWhere(sourceName, Field.List(columns), condition);
            }
            return commandText;
        }

        public IEnumerable<T> GetWhere<T>(ICondition condition)
        {
            var typeInfo = GetTypeInfo(typeof(T));
            return GetMany<T>(typeInfo.TableName, condition);
        }

        public T Create<T>(T instance)
        {
            var mapper = new TypeMapper();
            var typeInfo = mapper.GetTypeInfo(typeof (T));

            var values = mapper.GetInstanceValues(instance);

            if (typeInfo.HasPrimaryKey())
                values.Remove(typeInfo.PrimaryKey);

            var insert = dialect.Insert(typeInfo.TableName, values);
            ExecuteNonQuery(insert, 1);

            var identity = ExecuteScalar(dialect.SelectIdentity());

            if(typeInfo.HasPrimaryKey())
                mapper.SetValueOfPrimaryKey(instance, identity);

            return instance;
        }

        public void Update(object instance)
        {
            var mapper = new TypeMapper();
            var typeInfo = mapper.GetTypeInfo(instance.GetType());

            var values = mapper.GetInstanceValues(instance);

            if (typeInfo.HasPrimaryKey())
                values.Remove(typeInfo.PrimaryKey);

            var primaryKey = Convert.ToInt32(mapper.GetValueOfPrimaryKey(instance));
            var condition = Field.Equals(typeInfo.PrimaryKey, primaryKey);

            var insert = dialect.UpdateWhere(typeInfo.TableName, condition, values);
            ExecuteNonQuery(insert, 1);
        }

        public void Delete(object instance)
        {
            var mapper = new TypeMapper();
            var typeInfo = mapper.GetTypeInfo(instance.GetType());

            var primaryKey = Convert.ToInt32(mapper.GetValueOfPrimaryKey(instance));

            var condition = Field.Equals(typeInfo.PrimaryKey, primaryKey);
            var delete = dialect.DeleteWhere(typeInfo.TableName, condition);
            ExecuteNonQuery(delete, 1);
        }

        public void DeleteWhere<T>(ICondition condition)
        {
            var mapper = new TypeMapper();
            var typeInfo = mapper.GetTypeInfo(typeof(T));
            var delete = dialect.DeleteWhere(typeInfo.TableName, condition);
            ExecuteNonQuery(delete);
        }

        public void DeleteAll<T>()
        {
            var mapper = new TypeMapper();
            var typeInfo = mapper.GetTypeInfo(typeof(T));

            var delete = dialect.DeleteAll(typeInfo.TableName);
            ExecuteNonQuery(delete);
        }

        private IDbCommand CreateCommand(string commandText)
        {
            log.Info(commandText);

            var connection = transaction.Connection;
            var command = connection.CreateCommand();
            command.CommandText = commandText;
            command.Transaction = transaction;
            return command;
        }

        private void ExecuteNonQuery(string commandText)
        {
            ExecuteNonQuery(commandText, -1);
        }

        private void ExecuteNonQuery(string commandText, int expectedRowModificationCount)
        {
            var command = CreateCommand(commandText);
            var actualModificationCount = command.ExecuteNonQuery();

            if(actualModificationCount != expectedRowModificationCount && expectedRowModificationCount != -1)
                throw new InvalidRowCountException(expectedRowModificationCount, actualModificationCount);
        }

        private object ExecuteScalar(string commandText)
        {
            var command = CreateCommand(commandText);
            return command.ExecuteScalar();
        }

        private static IDataReader CreateLateBoundReader(IDbCommand command)
        {
            return new JustInTimeDataReader(command);
        }

        public void Rollback()
        {
            transaction.Rollback();
        }

        public void Commit()
        {
            transaction.Commit();
        }

        private static MappedTypeInfo GetTypeInfo(Type type)
        {
            var mapper = new TypeMapper();
            return mapper.GetTypeInfo(type);
        }
    }
}
