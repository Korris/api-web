using System.Data;
using System.Linq.Expressions;

namespace Mcsg.Common.Interfaces;

using Mcsg.Common.SeedWork.Responses;

public interface IRepository<TEntity>
{
    string TableName { get; set; }
    IDbConnection Connection { get; }

    Task<IEnumerable<TEntity>> GetAllAsync(bool newConection = false);

    Task<TEntity> GetByIdAsync(Guid id, string column = "", bool newConection = false);

    Task<IEnumerable<TEntity>> GetByPredicateAsync(Expression<Func<TEntity, bool>> predicate, string column = "", bool newConection = false);

    Task<TEntity> GetSignleByPredicateAsync(Expression<Func<TEntity, bool>> predicate, string column = "", bool newConection = false);

    Task<PagedResponse<TEntity>> GetByPageAsync(Expression<Func<TEntity, bool>> predicate, int pageNumber = 1, int pageSize = 10, string column = "", bool newConection = false);

    Task<int> InsertAsync(TEntity entity, bool newConection = false);

    Task<int> InsertAsync(List<TEntity> entities, bool newConection = false);

    Task<Guid> InsertEntityAsync(TEntity entity, bool newConection = false);

    Task<bool> UpdateAsync(TEntity entity, string primaryKey = "Id", bool newConection = false);

    Task<bool> DeleteAsync(Guid id, bool newConection = false);
    Task<bool> SoftDeleteAsync(Guid id, bool newConection = false);

    Task<IEnumerable<TEntity>> GetByCustomQuery(string query, object param = null, bool newConection = false);
}