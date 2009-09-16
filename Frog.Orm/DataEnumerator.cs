using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

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
                var properties = from c in typeof (T).GetProperties()
                                 where TypeMapper.GetAttribute<ColumnAttribute>(c) != null || TypeMapper.GetAttribute<PrimaryKeyAttribute>(c) != null
                                 select new {Property = c };

                var propertyList = properties.ToList();
                
                var dependencies = from c in typeof(T).GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
                                   where Attribute.GetCustomAttribute(c, typeof(RequiredDependencyAttribute)) != null
                                   select new { Property = c };
                
                var dependenciesList = dependencies.ToList();

                while (reader.Read())
                {
                    var instance = Activator.CreateInstance<T>();

                    dependenciesList.ForEach(x => x.Property.SetValue(instance, repositoryContext, null));

                    propertyList.ForEach(x =>
                                {
                                    var name = TypeMapper.GetColumnName(x.Property);

                                    var ordinal = reader.GetOrdinal(name);
                                    var value = reader.GetValue(ordinal);

                                    x.Property.SetValue(instance, TypeMapper.MapDbValueToDotNet(x.Property.PropertyType, value), null);
                                });

                    yield return instance;
                }
            }
        }
    }
}