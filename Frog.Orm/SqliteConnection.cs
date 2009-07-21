using System.Data;
using System.Data.SQLite;
using Frog.Orm.Dialects;

namespace Frog.Orm
{
    public class SqliteConnection : IConnection
    {
        private readonly SQLiteConnection connection;

        public SqliteConnection(string connectionString)
        {
            connection = new SQLiteConnection(connectionString);
        }

        public SqliteConnection(SQLiteConnection connection)
        {
            this.connection = connection;
        }

        public void Dispose()
        {
            if(connection.State == ConnectionState.Open)
                connection.Close();

            connection.Dispose();
        }

        public ITransaction GetTransaction()
        {
            if(connection.State != ConnectionState.Open)
                connection.Open();

            return new Transaction(connection.BeginTransaction(), new SqliteDialect(), DataEnumerator);
        }

        public DataEnumerator DataEnumerator { get; set; }
    }
}
