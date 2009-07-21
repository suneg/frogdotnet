using System;

namespace Frog.Orm
{
    public interface IConnection : IDisposable
    {
        ITransaction GetTransaction();
        DataEnumerator DataEnumerator { get; set; }
    }
}
