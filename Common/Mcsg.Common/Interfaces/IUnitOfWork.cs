using System.Data;

namespace Mcsg.Common.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IDbConnection Connection { get; }

    IRepository<TEntity> GetRepository<TEntity>();

    void BeginTransaction();

    void CommitTransaction();

    void RollbackTransaction();
}