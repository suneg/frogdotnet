using System.Collections.Generic;
using System.Data;

namespace Frog.Orm
{
    public interface IDataEnumerator
    {
        IEnumerable<T> GetEnumerator<T>(IDataReader reader);
    }
}