using System.Data;
using System.Data.SqlClient;
using Frog.Orm.Dialects;
using System.Data.SqlServerCe;

namespace Frog.Orm
{
    public class SqlServerCeConnection : IConnection
    {
        private readonly SqlCeConnection connection;
        private readonly ISqlDialect dialect;
        private ITransaction currentTransaction;
        private IDataEnumerator dataEnumerator;

        public SqlServerCeConnection(string connectionString)
        {
            connection = new SqlCeConnection(connectionString);
            dialect = new SqlCeDialect();
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
    }
}
