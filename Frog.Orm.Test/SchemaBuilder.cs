using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Text;

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
                    cmd.AppendFormat(",{0} {1}", column.Name, GetDbType(column.Type));
            }

            cmd.Append(")");

            command.CommandText = cmd.ToString();
            command.ExecuteNonQuery();            
        }

        private string GetDbType(Type type)
        {
            var booleanTypeName = "bit";

            if (this.connection.ConnectionString.Contains("sqlite"))
                booleanTypeName = "boolean";

            if (type.IsEnum)
                return "integer";
            if (type == typeof(Boolean))
                return booleanTypeName;
            if (type == typeof(Int32))
                return "integer";
            if (type == typeof(Decimal))
                return "decimal";
            if (type == typeof(Int64))
                return "long";
            if (type == typeof(String))
                return "nvarchar(64)";

            throw new NotImplementedException(String.Format("Unknown type. Cannot map '{0}' to database type", type.FullName));
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