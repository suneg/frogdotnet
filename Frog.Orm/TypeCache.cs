using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;


namespace Frog.Orm
{
    public class TypeCache
    {
        private static Dictionary<Type, SecondMappedTypeInfo> cache;

        public TypeCache()
        {
            if(cache == null)
                cache = new Dictionary<Type, SecondMappedTypeInfo>();
        }

        private SecondMappedTypeInfo Add(Type type)
        {
            var mappedTypeInfo = new SecondMappedTypeInfo();
            mappedTypeInfo.PrimaryKey = GetPrimaryKey(type);
            mappedTypeInfo.Columns = GetColumns(type);
            mappedTypeInfo.TableName = GetTableName(type);
            mappedTypeInfo.Dependencies = GetDependencies(type);

            cache.Add(type, mappedTypeInfo);
            return mappedTypeInfo;
        }

        private List<PropertyInfo> GetDependencies(Type type)
        {
            var result = new List<PropertyInfo>();

            var dependencies = from property in type.GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
                               where
                                   (Attribute.GetCustomAttribute(property, typeof (RequiredDependencyAttribute)) != null)
                               select property;


            result.AddRange(dependencies);
            return result;
        }

        private string GetTableName(Type type)
        {
            var attribute = ((TableAttribute)Attribute.GetCustomAttribute(type, typeof(TableAttribute)));

            if (attribute == null)
                throw new MappingException(String.Format("Cannot map type '{0}'. It has no [Table] annotation", type.Name));

            var overriddenName = attribute.Name;
            return overriddenName ?? type.Name;
        }

        private static List<SecondMappedColumnInfo> GetColumns(Type type)
        {
            var result = from property in type.GetProperties()
                         where (Attribute.GetCustomAttribute(property, typeof(ColumnAttribute)) != null ||
                                  Attribute.GetCustomAttribute(property, typeof(PrimaryKeyAttribute)) != null)
                         select new SecondMappedColumnInfo
                         {
                             Name = GetColumnName(property),
                             Info = property
                         };

            return result.ToList();
        }

        public SecondMappedTypeInfo GetTypeInfo(Type type)
        {
            SecondMappedTypeInfo mappedTypeInfo;

            if (!cache.TryGetValue(type, out mappedTypeInfo))
            {
                mappedTypeInfo = Add(type);
            }

            return mappedTypeInfo;
        }

        private SecondMappedColumnInfo GetPrimaryKey(Type type)
        {
            var primaryKeyProperty = type.GetProperties().FirstOrDefault(c => Attribute.GetCustomAttribute(c, typeof(PrimaryKeyAttribute)) != null);

            if (primaryKeyProperty == null)
                return null;

            var overriddenName = GetAttribute<PrimaryKeyAttribute>(primaryKeyProperty).Name;

            return new SecondMappedColumnInfo
            {
                Name = overriddenName ?? primaryKeyProperty.Name,
                Info = primaryKeyProperty
            };
        }


        public static T GetAttribute<T>(PropertyInfo property) where T : Attribute
        {
            return (T)Attribute.GetCustomAttribute(property, typeof(T));
        }

        public static string GetColumnName(PropertyInfo property)
        {
            var column = GetAttribute<ColumnAttribute>(property);

            var overriddenName = "";

            if (column != null)
                overriddenName = column.Name;
            else
            {
                var primaryKey = GetAttribute<PrimaryKeyAttribute>(property);
                overriddenName = primaryKey.Name;
            }

            return overriddenName ?? property.Name;
        }
    }
}
