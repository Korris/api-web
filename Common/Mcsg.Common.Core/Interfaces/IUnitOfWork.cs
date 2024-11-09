using System.Data;

namespace Mcsg.Lib.Data.Repositories.Interface;

public interface IUnitOfWork : IDisposable
{
    IDbConnection Connection { get; }

    IRepository<TEntity> GetRepository<TEntity>();

    void BeginTransaction();

    void CommitTransaction();

    void RollbackTransaction();
}