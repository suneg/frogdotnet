using System.Data;
using System.Data.SqlClient;
using Frog.Orm.Dialects;

namespace Frog.Orm
{
    public class SqlServerConnection : IConnection
    {
        private readonly SqlConnection connection;

        public SqlServerConnection(string connectionString)
        {
            connection = new SqlConnection(connectionString);
        }

        public void Dispose()
        {
            if (connection.State == ConnectionState.Open)
                connection.Close();

            connection.Dispose();
        }

        public ITransaction BeginTransaction()
        {
            if(connection.State == ConnectionState.Closed)
                connection.Open();

            return new Transaction(connection.BeginTransaction(), new TransactSqlDialect(), DataEnumerator);
        }

        public DataEnumerator DataEnumerator { get; set; }
    }
}
