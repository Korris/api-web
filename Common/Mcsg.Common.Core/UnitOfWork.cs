using Microsoft.Extensions.DependencyInjection;
using System.Data;

namespace Mcsg.Lib.Data.Repositories;

using Interface;

public class UnitOfWork : IUnitOfWork
{
    public IDbConnection Connection { get; }

    private readonly IServiceProvider _serviceProvider;
    private IDbTransaction _dbTransaction;

    public UnitOfWork(IServiceProvider serviceProvider, IDbConnection connection)
    {
        Connection = connection;
        _serviceProvider = serviceProvider;
    }

    public IRepository<TEntity> GetRepository<TEntity>()
    {
        return _serviceProvider.GetService<IRepository<TEntity>>() ?? throw new NotImplementedException($"Not implemented the type {typeof(IRepository<TEntity>)}");
    }

    public void BeginTransaction()
    {
        if (Connection.State != ConnectionState.Open)
            Connection.Open();
        _dbTransaction = Connection.BeginTransaction();
    }

    public void CommitTransaction()
    {
        _dbTransaction.Commit();
    }

    public void RollbackTransaction()
    {
        _dbTransaction.Rollback();
    }

    public void Dispose()
    {
        Connection.Dispose();
    }
}