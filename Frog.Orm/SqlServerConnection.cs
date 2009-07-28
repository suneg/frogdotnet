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

        public SqlServerConnection(string connectionString)
        {
            connection = new SqlConnection(connectionString);
            dialect = new TransactSqlDialect();
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

            currentTransaction = new Transaction(connection.BeginTransaction(), Dialect, DataEnumerator);
            return currentTransaction;
        }

        /// <summary>
        /// Rollback changes made in the current transaction
        /// </summary>
        public void Rollback()
        {
            currentTransaction.Rollback();   // TODO: Fail if no transaction is running
            currentTransaction = null;
        }

        public ISqlDialect Dialect
        {
            get { return dialect; }
        }

        /// <summary>
        /// Commit changes made in the current transaction
        /// </summary>
        public void CommitChanges()
        {
            currentTransaction.Commit();     // TODO: Fail if no transaction is running
            currentTransaction = null;
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

        public DataEnumerator DataEnumerator { get; set; }
    }
}
