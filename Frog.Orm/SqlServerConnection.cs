using System.Data;
using System.Data.SqlClient;
using Frog.Orm.Dialects;

namespace Frog.Orm
{
    public class SqlServerConnection : IConnection
    {
        private readonly SqlConnection connection;
        private readonly ISqlDialect dialect;
        private ITransaction currentTransaction;
        private IDataEnumerator dataEnumerator;

        public SqlServerConnection(string connectionString)
        {
            connection = new SqlConnection(connectionString);
            dialect = new TransactSqlDialect();
            IsolationLevel = IsolationLevel.Unspecified;
        }

        public void Dispose()
        {
            if (currentTransaction != null)
            {
                currentTransaction.Rollback();
                currentTransaction = null;
            }

            if (connection.State == ConnectionState.Open)
                connection.Close();

            connection.Dispose();
        }

        private ITransaction BeginTransaction()
        {
            if(connection.State == ConnectionState.Closed)
                connection.Open();

            currentTransaction = new Transaction(connection.BeginTransaction(IsolationLevel), Dialect, DataEnumerator);
            return currentTransaction;
        }

        /// <summary>
        /// Rollback changes made in the current transaction
        /// </summary>
        public void Rollback()
        {
            if (currentTransaction != null)
            {
                currentTransaction.Rollback();
                currentTransaction = null;
            }
        }

        /// <summary>
        /// Commit changes made in the current transaction
        /// </summary>
        public void CommitChanges()
        {
            if (currentTransaction != null)
            {
                currentTransaction.Commit();
                currentTransaction = null;
            }
        }

        /// <summary>
        /// Gets the currently active transaction. If none exists a new will be created.
        /// </summary>
        public ITransaction Transaction
        {
            get
            {
                if (currentTransaction == null)
                    currentTransaction = BeginTransaction();

                return currentTransaction;
            }
        }

        public ISqlDialect Dialect
        {
            get { return dialect; }
        }

        public IDataEnumerator DataEnumerator
        {
            get
            {
                return dataEnumerator;
            }
            set
            {
                dataEnumerator = value;

                if (currentTransaction != null)
                    currentTransaction.InitializeDataEnumerator(dataEnumerator);
            }
        }

        /// <summary>
        /// Sets or gets the isolation level for the next transaction. Is Unspecified by default using the SQLServer default
        /// </summary>
        public IsolationLevel IsolationLevel { get; set; }
    }
}
