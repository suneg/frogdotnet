using System.Data;
using System.Data.SQLite;
using Frog.Orm.Dialects;

namespace Frog.Orm
{
    public class SqliteConnection : IConnection
    {
        private readonly SQLiteConnection connection;
        private readonly ISqlDialect dialect;
        private ITransaction currentTransaction;
        private IDataEnumerator dataEnumerator;


        public SqliteConnection(string connectionString)
        {
            connection = new SQLiteConnection(connectionString);
            dialect = new SqliteDialect();
        }

        public SqliteConnection(SQLiteConnection connection)
        {
            this.connection = connection;
            dialect = new SqliteDialect();
        }

        public void Dispose()
        {
            if (currentTransaction != null)
            {
                currentTransaction.Rollback();
                currentTransaction = null;
            }

            if(connection.State == ConnectionState.Open)
                connection.Close();

            connection.Dispose();
        }

        public void Rollback()
        {
            currentTransaction.Rollback(); // TODO: Fail or ignore if no transaction is running
        }

        /// <summary>
        /// Commit changes in the repository to the underlying data source.
        /// </summary>
        public void CommitChanges()
        {
            currentTransaction.Commit();
            currentTransaction = null;
        }

        private ITransaction BeginTransaction()
        {
            if(connection.State != ConnectionState.Open)
                connection.Open();

            return new Transaction(connection.BeginTransaction(), Dialect, DataEnumerator);
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

        public ISqlDialect Dialect { get { return dialect; } }
        
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
