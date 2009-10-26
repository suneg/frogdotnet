using System;
using System.Reflection;

namespace Frog.Orm
{
    public class MappedColumnInfo
    {
        public string Name { get; set; }
        public Type Type { get; set; }
    }

    public class SecondMappedColumnInfo
    {
        public string Name { get; set; }
        public PropertyInfo Info { get; set; }
    }
}