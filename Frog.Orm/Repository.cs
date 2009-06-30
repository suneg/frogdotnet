using System;
using System.Collections.Generic;
using Frog.Orm.Conditions;

namespace Frog.Orm
{
    public class Repository : IRepository
    {
        private readonly IConnection connection;
        private ITransaction transaction;

        public Repository(IConnection connection)
        {
            this.connection = connection;
            connection.DataEnumerator = new DataEnumerator(this);
        }

        /// <summary>
        /// Disposes resources used by the repository, including the underlying database connection.
        /// </summary>
        public void Dispose()
        {
            if (transaction != null)
            {
                transaction.Rollback();        
            }

            connection.Dispose();
        }

        public T Get<T>(long primaryKeyValue)
        {
            return Transaction.GetByPrimaryKey<T>(primaryKeyValue);
        }

        public T Get<T>(Guid primaryKeyValue)
        {
            return Transaction.GetByPrimaryKey<T>(primaryKeyValue);
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
        /// Gets all entities of type T
        /// </summary>
        /// <example>GetAll&lt;Customer&gt;()</example>
        /// <typeparam name="T"></typeparam>
        /// <returns>Enumerable set of entities</returns>
        public IEnumerable<T> GetAll<T>()
        {
            return Transaction.GetAll<T>();
        }

        public IEnumerable<T> GetWhere<T>(ICondition condition)
        {
            return Transaction.GetWhere<T>(condition);
        }

        /// <summary>
        /// Commit changes in the repository to the underlying data source.
        /// </summary>
        public void CommitChanges()
        {
            Transaction.Commit();
            transaction = null;
        }

        public T Create<T>(T obj)
        {
            return Transaction.Create(obj);
        }

        public void Remove(object obj)
        {
            Transaction.Delete(obj);
        }

        public void Update(object obj)
        {
            Transaction.Update(obj);
        }

        private ITransaction Transaction
        {
            get
            {
                if(transaction == null)
                    transaction = connection.BeginTransaction();

                return transaction;
            }
        }
    }
}
