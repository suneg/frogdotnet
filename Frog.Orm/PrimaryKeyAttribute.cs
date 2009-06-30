using System;

namespace Frog.Orm
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class PrimaryKeyAttribute : Attribute
    {
        public string Name { get; set; }
    }
}
