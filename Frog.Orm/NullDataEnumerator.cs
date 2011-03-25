using System;
using System.Collections.Generic;
using System.Data;

namespace Frog.Orm
{
    public class NullDataEnumerator : IDataEnumerator
    {
        public IEnumerable<T> GetEnumerator<T>(IDataReader reader)
        {
            throw new InvalidOperationException("This is a null object");
        }
    }
}