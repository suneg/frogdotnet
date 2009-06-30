using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Text;
using Frog.Orm;

namespace Frog.Orm.Test
{
    internal class SchemaBuilder
    {
        private readonly DbConnection connection;

        public SchemaBuilder(DbConnection connection)
        {
            this.connection = connection;

            if(connection.State != ConnectionState.Open)
                connection.Open();
        }

        public void CreateTableFromType<T>()
        {
            var command = connection.CreateCommand();

            var mapper = new TypeMapper();
            var info = mapper.GetTypeInfo(typeof(T));

            command.CommandType = CommandType.Text;
            

            var cmd = new StringBuilder();
            cmd.AppendFormat("create table [{0}](", info.TableName);

            if (connection.GetType() == typeof(SqlConnection))
                cmd.AppendFormat("id INTEGER identity");

            if (connection.GetType() == typeof(SQLiteConnection))
                cmd.AppendFormat("id INTEGER PRIMARY KEY AUTOINCREMENT");

            foreach (var column in info.Columns)
            {
                if (column.Name != info.PrimaryKey)
                    cmd.AppendFormat(",{0} varchar(64)", column.Name);
            }

            cmd.Append(")");

            command.CommandText = cmd.ToString();
            command.ExecuteNonQuery();            
        }

        public void CreateViewFromType<T>(string viewName)
        {
            var command = connection.CreateCommand();

            var mapper = new TypeMapper();
            var info = mapper.GetTypeInfo(typeof(T));

            command.CommandType = CommandType.Text;


            var cmd = new StringBuilder();
            cmd.AppendFormat("create view [{0}] as ", viewName);
            cmd.AppendFormat("select Id from [{0}]", info.TableName);

            command.CommandText = cmd.ToString();
            command.ExecuteNonQuery();
        }


        //private string GetDbType(Type type)
        //{
        //    if (type == typeof(Int32))
        //        return "integer";
        //    if (type == typeof(Int64))
        //        return "long";
        //    if (type  == typeof(String))
        //        return "nvarchar";

        //    throw new NotImplementedException("Unknown type. Cannot map to database type");
        //}
        public void RemoveView(string viewName)
        {
            var command = connection.CreateCommand();
            command.CommandType = CommandType.Text;

            var cmd = new StringBuilder();
            cmd.AppendFormat("drop view [{0}]", viewName);

            command.CommandText = cmd.ToString();
            command.ExecuteNonQuery();
        }

        public void RemoveTableFromType<T>()
        {
            var command = connection.CreateCommand();
            command.CommandType = CommandType.Text;

            var mapper = new TypeMapper();
            var info = mapper.GetTypeInfo(typeof(T));

            var cmd = new StringBuilder();
            cmd.AppendFormat("drop table [{0}]", info.TableName);

            command.CommandText = cmd.ToString();
            command.ExecuteNonQuery();

        }
    }
}