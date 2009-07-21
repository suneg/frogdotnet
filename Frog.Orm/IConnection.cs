using System;

namespace Frog.Orm
{
    public interface IConnection : IDisposable
    {
        ITransaction Transaction { get; }
        DataEnumerator DataEnumerator { get; set; }
        void CommitChanges();
        void Rollback();
    }
}
