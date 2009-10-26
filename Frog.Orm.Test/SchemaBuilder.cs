using System;
using System.Text;

namespace Frog.Orm.Test
{
    internal class SchemaBuilder
    {
        private readonly IConnection connection;
        private TypeMapper mapper;

        public SchemaBuilder(IConnection connection)
        {
            this.connection = connection;
        }

        public void CreateTableFromType<T>()
        {
            mapper = new TypeMapper();
            var info = this.mapper.GetTypeInfo(typeof(T));            

            var cmd = new StringBuilder();
            cmd.AppendFormat("create table [{0}](", info.TableName);

            if (connection is SqlServerConnection)
                cmd.AppendFormat("id INTEGER identity");

            if (connection is SqliteConnection)
                cmd.AppendFormat("id INTEGER PRIMARY KEY AUTOINCREMENT");

            foreach (var column in info.Columns)
            {
                if (!info.HasPrimaryKey() || column.Name != info.PrimaryKey.Name)
                    cmd.AppendFormat(",{0} {1}", column.Name, GetDbType(column.Info.PropertyType));
            }

            cmd.Append(")");

            connection.Transaction.ExecuteRaw(cmd.ToString());
        }

        private string GetDbType(Type type)
        {
            var booleanTypeName = "bit";
            var decimalTypeName = "decimal";

            if (connection is SqliteConnection)
            {
                decimalTypeName = "real";
                booleanTypeName = "boolean";
            }

            if (type.IsEnum)
                return "integer";
            if (type == typeof(Boolean))
                return booleanTypeName;
            if (type == typeof(Int32))
                return "integer";
            if (type == typeof(Decimal))
                return decimalTypeName;
            if (type == typeof(Int64))
                return "long";
            if (type == typeof(String))
                return "nvarchar(64)";
            if (type == typeof(Double))
                return "float";

            throw new NotImplementedException(String.Format("Unknown type. Cannot map '{0}' to database type", type.FullName));
        }

        public void CreateViewFromType<T>(string viewName)
        {
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
            var info = mapper.GetTypeInfo(typeof(T));

            var cmd = new StringBuilder();
            cmd.AppendFormat("drop table [{0}]", info.TableName);

            connection.Transaction.ExecuteRaw(cmd.ToString());
        }
    }
}