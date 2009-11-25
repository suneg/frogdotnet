using System;
using System.Collections.Generic;
using System.Reflection;

namespace Frog.Orm
{
    public class TypeMapper
    {
        private readonly TypeCache typeCache;

        public TypeMapper()
        {
            typeCache = new TypeCache();
        }

        public MappedTypeInfo GetTypeInfo(Type type)
        {
            return typeCache.GetTypeInfo(type);
        }

        public object GetValueOfPrimaryKey(object instance)
        {
            var info = typeCache.GetTypeInfo(instance.GetType());

            if (info.PrimaryKey == null)
                return null;

            return info.PrimaryKey.Info.GetValue(instance, null);  // TODO: What if missing primary key?
        }

        public void SetValueOfPrimaryKey(object instance, object identity)
        {
            var info = typeCache.GetTypeInfo(instance.GetType());
            PropertyInfo primaryKeyProperty = info.PrimaryKey.Info;

            primaryKeyProperty.SetValue(instance, MapDbValueToDotNet(primaryKeyProperty.PropertyType, identity), null); // TODO: What if missing primary key?
        }

        public Dictionary<string, object> GetInstanceValues(object instance)
        {
            var result = new Dictionary<string, object>();

            var typeInfo = typeCache.GetTypeInfo(instance.GetType());

            foreach (var column in typeInfo.Columns)
            {
                result.Add(column.Name, column.Info.GetValue(instance, null));
            }

            return result;
        }

        public static object MapDbValueToDotNet(Type targetType, object value)
        {
            if (targetType.IsEnum)
                return Convert.ToInt32(value);

            if (targetType.Equals(typeof(Double)))
                return Convert.ToDouble(value);

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
    }
}
