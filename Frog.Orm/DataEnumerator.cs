using System;
using System.Collections.Generic;
using System.Data;

namespace Frog.Orm
{
    public class DataEnumerator : IDataEnumerator
    {
        private readonly IRepository repositoryContext;

        public DataEnumerator(IRepository repositoryContext)
        {
            this.repositoryContext = repositoryContext;
        }

        public IEnumerable<T> GetEnumerator<T>(IDataReader reader)
        {
            using (reader)
            {
                var typeMapper = new TypeMapper();
                var info = typeMapper.GetTypeInfo(typeof(T));

                while (reader.Read())
                {
                    var instance = Activator.CreateInstance<T>();

                    foreach (var dependency in info.Dependencies)
                    {
                        dependency.SetValue(instance, repositoryContext, null);
                    }

                    foreach(var column in info.Columns)
                    {
                        var ordinal = reader.GetOrdinal(column.Name);
                        var value = reader.GetValue(ordinal);

                        column.Info.SetValue(instance, TypeMapper.MapDbValueToDotNet(column.Info.PropertyType, value), null);
                    }

                    yield return instance;
                }
            }
        }
    }
}