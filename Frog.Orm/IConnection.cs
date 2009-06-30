using System;

namespace Frog.Orm
{
    public interface IConnection : IDisposable
    {
        ITransaction BeginTransaction();
        DataEnumerator DataEnumerator { get; set; }
    }
}
