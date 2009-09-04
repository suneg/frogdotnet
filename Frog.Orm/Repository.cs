using System;
using System.Collections.Generic;
using Frog.Orm.Conditions;

namespace Frog.Orm
{
    public class Repository : IRepository
    {
        private readonly IConnection connection;

        public Repository(IConnection connection)
        {
            this.connection = connection;
            connection.DataEnumerator = new DataEnumerator(this);
        }

        /// <summary>
        /// Get a record from the database based on its identity (primary key)
        /// </summary>
        /// <typeparam name="T">The mapping type of the record</typeparam>
        /// <param name="primaryKeyValue">The long/bigint identity value of the record</param>
        /// <returns>A mapped instance of the record</returns>
        public T Get<T>(long primaryKeyValue)
        {
            return Transaction.GetByPrimaryKey<T>(primaryKeyValue);
        }

        /// <summary>
        /// Get a record from the database based on its identity (primary key)
        /// </summary>
        /// <typeparam name="T">The mapping type of the record</typeparam>
        /// <param name="primaryKeyValue">The guid identity value of the record</param>
        /// <returns>A mapped instance of the record</returns>
        public T Get<T>(Guid primaryKeyValue)
        {
            return Transaction.GetByPrimaryKey<T>(primaryKeyValue);
        }        

        /// <summary>
        /// Gets all entities of type T
        /// </summary>
        /// <example>GetAll&lt;Customer&gt;()</example>
        /// <typeparam name="T"></typeparam>
        /// <returns>Enumerable set of entities</returns>
        public IEnumerable<T> GetAll<T>()
        {
            return Transaction.GetAll<T>();
        }

        // TODO: public IEnumerable<T> GetAll<T>(Order)

        /// <summary>
        /// Gets a set of entities of type T that matches a supplied criteria.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="condition">Condition that must be met for an entity to be included in the result.</param>
        /// <returns></returns>
        public IEnumerable<T> GetWhere<T>(ICondition condition)
        {
            return Transaction.GetWhere<T>(condition);
        }

        // TODO: public IEnumerable<T> GetWhere<T>(Condition, Order)

        public T Create<T>(T obj)
        {
            return Transaction.Create(obj);
        }

        /// <summary>
        /// Deletes a record in the database.
        /// </summary>
        /// <example>Remove&lt;Product&gt;(selectedProduct);</example>
        public void Remove(object obj)
        {
            Transaction.Delete(obj);
        }

        /// <summary>
        /// Deletes all entities of type T, where the supplied condition is met.
        /// </summary>
        /// <example>RemoveWhere&lt;Customer&gt;(Field.Equals("FirstName", "Sune"));</example>
        /// <typeparam name="T"></typeparam>
        public void RemoveWhere<T>(ICondition condition)
        {
            Transaction.DeleteWhere<T>(condition);
        }

        /// <summary>
        /// Deletes all entities of type T
        /// </summary>
        /// <typeparam name="T">The type of records to remove</typeparam>
        public void RemoveAll<T>()
        {
            Transaction.DeleteAll<T>();
        }

        /// <summary>
        /// Updates the record in the underlying database that corresponds to the supplied instance.
        /// </summary>
        public void Update(object obj)
        {
            Transaction.Update(obj);
        }

        /// <summary>
        /// Retrieves a list of rows for either a table or view in the database.
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <param name="sourceName">Name of a Table or View in the database</param>
        /// <param name="condition">Adds a condition to filter the result</param>
        /// <returns>Enumerable list of records mapped to a .NET entity</returns>
        protected IEnumerable<T> GetMany<T>(string sourceName, ICondition condition/*, IOrdering order*/)
        {
            return Transaction.GetMany<T>(sourceName, condition);
        }

        /// <summary>
        /// Gets a single entity from a table or view in the database
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <param name="sourceName">Name of a Table or View in the database</param>
        /// <param name="condition">Adds a condition to filter the result</param>
        /// <returns>A database record mapped to a .NET entity</returns>
        protected T GetSingle<T>(string sourceName, ICondition condition)
        {
            return Transaction.GetSingle<T>(sourceName, condition);
        }

        /// <summary>
        /// Executes an arbitrary SQL statement, and attempts conversion of resulting rows into instances of the specified mapping class (T).
        /// </summary>
        /// <typeparam name="T">The mapping class which to convert the results to</typeparam>
        /// <param name="rawSqlStatement">Actual SQL statement to be executed against the underlying database</param>
        protected IEnumerable<T> ExecuteRawSql<T>(string rawSqlStatement)
        {
            return Transaction.ExecuteRaw<T>(rawSqlStatement);
        }

        /// <summary>
        /// Executes a statement that produces a single-cell result.
        /// </summary>
        /// <param name="scalarExpression"></param>
        /// <returns>The single value returned by the database</returns>
        protected object GetScalar(IScalarExpression scalarExpression)
        {
            return Transaction.GetScalar(scalarExpression);
        }

        private ITransaction Transaction
        {
            get
            {
                return connection.Transaction;
            }
        }
    }
}
