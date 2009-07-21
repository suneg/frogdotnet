using System.Data;
using System.Data.SqlClient;
using Frog.Orm.Dialects;

namespace Frog.Orm
{
    public class SqlServerConnection : IConnection
    {
        private readonly SqlConnection connection;
        private ITransaction currentTransaction;

        public SqlServerConnection(string connectionString)
        {
            connection = new SqlConnection(connectionString);
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

            currentTransaction = new Transaction(connection.BeginTransaction(), new TransactSqlDialect(), DataEnumerator);
            return currentTransaction;
        }

        /// <summary>
        /// Rollback changes made in the current transaction
        /// </summary>
        public void Rollback()
        {
            currentTransaction.Rollback();   // TODO: Fail if no transaction is running
        }

        /// <summary>
        /// Commit changes made in the current transaction
        /// </summary>
        public void CommitChanges()
        {
            currentTransaction.Commit();     // TODO: Fail if no transaction is running
            currentTransaction = null;
        }

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
