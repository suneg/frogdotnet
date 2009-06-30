using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Frog.Orm
{
    public class TypeMapper
    {
        public MappedTypeInfo GetTypeInfo(Type type)
        {
            var typeInfo = new MappedTypeInfo();

            var attribute = ((TableAttribute)Attribute.GetCustomAttribute(type, typeof(TableAttribute)));

            if(attribute == null)
                throw new MappingException(String.Format("Cannot map type '{0}'. It has no [Table] annotation", type.Name));

            var overriddenName = attribute.Name;
            var primaryKey = GetPrimaryKey(type);

            typeInfo.TableName = overriddenName ?? type.Name;
            typeInfo.PrimaryKey = primaryKey != null ? primaryKey.Name : null;

            if(primaryKey != null)
                typeInfo.Columns.Add(primaryKey);

            typeInfo.Columns.AddRange(GetColumns(type));

            return typeInfo;
        }

        private MappedColumnInfo GetPrimaryKey(Type type)
        {
            var primaryKeyProperty = type.GetProperties().FirstOrDefault(c => Attribute.GetCustomAttribute(c, typeof(PrimaryKeyAttribute)) != null);

            if (primaryKeyProperty == null)
                return null;

            var overriddenName = GetAttribute<PrimaryKeyAttribute>(primaryKeyProperty).Name;

            return new MappedColumnInfo
                    {
                        Name = overriddenName ?? primaryKeyProperty.Name,
                        Type = GetColumnType(primaryKeyProperty)
                    };
        }

        private static IEnumerable<MappedColumnInfo> GetColumns(Type type)
        {
            return from property in type.GetProperties()
                   where Attribute.GetCustomAttribute(property, typeof (ColumnAttribute)) != null
                   select new MappedColumnInfo
                              {
                                  Name = GetColumnName(property),
                                  Type = GetColumnType(property)
                              };
        }

        private static Type GetColumnType(PropertyInfo property)
        {
            return property.PropertyType;
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

        public static T GetAttribute<T>(PropertyInfo property) where T : Attribute
        {
            return (T)Attribute.GetCustomAttribute(property, typeof(T));
        }

        public Dictionary<string, object> GetInstanceValues(object instance)
        {
            var result = new Dictionary<string, object>();

            var props = from c in instance.GetType().GetProperties()
                        where
                            Attribute.GetCustomAttribute(c, typeof (ColumnAttribute)) != null ||
                            Attribute.GetCustomAttribute(c, typeof (PrimaryKeyAttribute)) != null
                        select new {Name = GetColumnName(c), Value = c.GetValue(instance, null) };


            props.ToList().ForEach(x => result.Add(x.Name, x.Value));

            return result;
        }

        public void SetValueOfPrimaryKey(object instance, object value)
        {
            var props = from c in instance.GetType().GetProperties()
                        where
                            Attribute.GetCustomAttribute(c, typeof (PrimaryKeyAttribute)) != null
                        select c;

            props.ToList().ForEach(c => c.SetValue(instance, MapDbValueToDotNet(c.PropertyType, value), null));
        }

        public static object MapDbValueToDotNet(Type targetType, object value)
        {
            if (targetType.IsEnum)
                return Convert.ToInt32(value);

            if (targetType.Equals(typeof(Decimal)))
                return Convert.ToDecimal(value);

            if (targetType.Equals(typeof(Int32)))
                return Convert.ToInt32(value);

            if (targetType.Equals(typeof(Int64)))
                return Convert.ToInt64(value);

            if (targetType.Equals(typeof(Boolean)))
                return Convert.ToBoolean(value);

            if (value is DBNull)
                return null;

            return value;
        }

        public object GetValueOfPrimaryKey(object instance)
        {
            var props = from c in instance.GetType().GetProperties()
                        where
                            Attribute.GetCustomAttribute(c, typeof(PrimaryKeyAttribute)) != null
                        select c;

            var first = props.ToList().FirstOrDefault();

            if(first == null)
                return null;

            return first.GetValue(instance, null);  // TODO: What if missing primary key
        }
    }
}
