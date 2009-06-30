using System;

namespace Frog.Orm
{
    /// <summary>
    /// Attribute used to decorate the properties that are filled with
    /// the result from the database.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class ColumnAttribute : Attribute
    {
        /// <summary>
        /// Name of the column in the database.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Allows the column value to be set to the result of a database function (e.g. sysdate)
        /// </summary>
        public string AlwaysUpdateTo { get; set; }
    }
}