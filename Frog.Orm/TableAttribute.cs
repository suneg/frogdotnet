using System;

namespace Frog.Orm
{
    /// <summary>
    /// Attribute used to decorate the properties that are filled with
    /// the result from the database.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class TableAttribute : Attribute
    {
        /// <summary>
        /// Name of the table in the database.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// If this table only has one row, we can update it without a primarykey.
        /// Otherwise, it will fail.
        /// </summary>
        public bool LonelyRow { get; set; }
    }
}
