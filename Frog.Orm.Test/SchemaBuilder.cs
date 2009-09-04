using System;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Text;

namespace Frog.Orm.Test
{
    internal class SchemaBuilder
    {
        private readonly IConnection connection;

        public SchemaBuilder(IConnection connection)
        {
            this.connection = connection;
        }

        public void CreateTableFromType<T>()
        {
            var mapper = new TypeMapper();
            var info = mapper.GetTypeInfo(typeof(T));            

            var cmd = new StringBuilder();
            cmd.AppendFormat("create table [{0}](", info.TableName);

            if (connection is SqlServerConnection)
                cmd.AppendFormat("id INTEGER identity");

            if (connection is SqliteConnection)
                cmd.AppendFormat("id INTEGER PRIMARY KEY AUTOINCREMENT");

            foreach (var column in info.Columns)
            {
                if (column.Name != info.PrimaryKey)
                    cmd.AppendFormat(",{0} {1}", column.Name, GetDbType(column.Type));
            }

            cmd.Append(")");

            connection.Transaction.ExecuteRaw(cmd.ToString());
        }

        private string GetDbType(Type type)
        {
            var booleanTypeName = "bit";

            if (connection is SqliteConnection)
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
            var mapper = new TypeMapper();
            var info = mapper.GetTypeInfo(typeof(T));

            var cmd = new StringBuilder();
            cmd.AppendFormat("create view [{0}] as ", viewName);
            cmd.AppendFormat("select Id from [{0}]", info.TableName);

            connection.Transaction.ExecuteRaw(cmd.ToString());
        }

        public void RemoveView(string viewName)
        {
            var cmd = new StringBuilder();
            cmd.AppendFormat("drop view [{0}]", viewName);

            connection.Transaction.ExecuteRaw(cmd.ToString());
        }

        public void RemoveTableFromType<T>()
        {
            var mapper = new TypeMapper();
            var info = mapper.GetTypeInfo(typeof(T));

            var cmd = new StringBuilder();
            cmd.AppendFormat("drop table [{0}]", info.TableName);

            connection.Transaction.ExecuteRaw(cmd.ToString());
        }
    }
}