using System.Collections.Generic;
using System.Reflection;

namespace Frog.Orm
{
    public class MappedTypeInfo
    {
        public MappedTypeInfo()
        {
            Columns = new List<MappedColumnInfo>();
        }

        public List<MappedColumnInfo> Columns { get; set; }
        public string TableName { get; set; }
        public string PrimaryKey { get; set; }

        public bool HasPrimaryKey()
        {
            return PrimaryKey != null;
        }
    }

    public class SecondMappedTypeInfo
    {
        public SecondMappedTypeInfo()
        {
            Columns = new List<SecondMappedColumnInfo>();
        }

        public string TableName { get; set; }
        public List<PropertyInfo> Dependencies { get; set; }
        public SecondMappedColumnInfo PrimaryKey { get; set; }
        public List<SecondMappedColumnInfo> Columns { get; set; }        

        public bool HasPrimaryKey()
        {
            return PrimaryKey != null;
        }
    }
}