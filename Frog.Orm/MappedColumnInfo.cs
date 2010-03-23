using System.Reflection;

namespace Frog.Orm
{
    public class MappedColumnInfo
    {
        public string Name { get; set; }
        public PropertyInfo Info { get; set; }
    }
}