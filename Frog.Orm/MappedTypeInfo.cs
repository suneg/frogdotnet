using System.Collections.Generic;

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
}