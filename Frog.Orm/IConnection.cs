using System;
using Frog.Orm.Dialects;

namespace Frog.Orm
{
    public interface IConnection : IDisposable
    {
        ITransaction Transaction { get; }
        DataEnumerator DataEnumerator { get; set; }
        ISqlDialect Dialect { get; }
        void CommitChanges();
        void Rollback();
    }
}
