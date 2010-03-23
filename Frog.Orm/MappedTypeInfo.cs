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

        public string TableName { get; set; }
        public List<PropertyInfo> Dependencies { get; set; }
        public MappedColumnInfo PrimaryKey { get; set; }
        public List<MappedColumnInfo> Columns { get; set; }        

        public bool HasPrimaryKey()
        {
            return PrimaryKey != null;
        }
    }
}