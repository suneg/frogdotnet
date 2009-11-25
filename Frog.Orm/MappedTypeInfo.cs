using System.Collections.Generic;
using System.Reflection;

namespace Frog.Orm
{
    public class MappedTypeInfo
    {
        public MappedTypeInfo()
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