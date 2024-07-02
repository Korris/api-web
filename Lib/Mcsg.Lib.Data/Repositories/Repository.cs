using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using static Dapper.SqlMapper;

namespace Mcsg.Lib.Data.Repositories
{
    using Common.SeedWork.Extensions;
    using Constants;
    using Entities.Common;
    using Extensions;

    public partial class Repository<TEntity> : IRepository<TEntity>
    {
        private readonly IConfiguration _configuration;
        public IDbConnection Connection { get; }

        public Repository(IConfiguration configuration, IDbConnection connection)
        {
            _configuration = configuration;
            SqlMapper.AddTypeHandler(new DateOnlyTypeHandler());
            Connection = connection;

            // Determine the table to be used
            var table = ((TableAttribute?)typeof(TEntity).GetCustomAttribute(typeof(TableAttribute)))?.Name;
            table = table == null ? EntityName.ToPlural() : table;

            // Determine the schema to be used
            var dbSchema = DbSchema.Default;
            var tables = DbSchema.IdentityTables.Split(';');
            if (tables.Contains(table))
            {
                dbSchema = $"{DbSchema.Identity}.";
            }

            _tableName = $"{dbSchema}\"{table}\"";
        }

        private static string EntityName => typeof(TEntity).Name;
        private string _tableName;

        public string? TableName
        {
            get => _tableName;
            set => _tableName = value;
        }

        protected string StandardizedQuery(string column = "")
        {
            if (string.IsNullOrEmpty(column))
                return $"SELECT * FROM {TableName}";
            return $"SELECT {column} FROM {TableName}";
        }
        protected IDbConnection CreateConnection()
        {
            var connection = new NpgsqlConnection(Connection.ConnectionString);
            connection.Open();
            return connection;
        }

        public virtual async Task<IEnumerable<TEntity>> GetAllAsync(bool newConection = false)
        {
            if (newConection)
            {
                using var connection = CreateConnection();
                return await connection.QueryAsync<TEntity>(StandardizedQuery());
            }
            return await Connection.QueryAsync<TEntity>(StandardizedQuery());
        }

        public virtual async Task<TEntity> GetByIdAsync(Guid id, string column = "", bool newConection = false)
        {
            var query = $"{StandardizedQuery(column)} WHERE \"Id\" = @Id";
            if (newConection)
            {
                using var connection = CreateConnection();
                return await connection.QueryFirstOrDefaultAsync<TEntity>(query, new { Id = id });
            }
            return await Connection.QueryFirstOrDefaultAsync<TEntity>(query, new { Id = id });
        }

        public virtual async Task<IEnumerable<TEntity>> GetByPredicateAsync(Expression<Func<TEntity, bool>> predicate, string column = "", bool newConection = false)
        {
            var query = $"{StandardizedQuery(column)} WHERE {GetSqlFromPredicate(predicate)}";
            if (newConection)
            {
                using var connection = CreateConnection();
                return await connection.QueryAsync<TEntity>(query);
            }
            return await Connection.QueryAsync<TEntity>(query);
        }

        public virtual async Task<TEntity> GetSignleByPredicateAsync(Expression<Func<TEntity, bool>> predicate, string column = "", bool newConection = false)
        {
            var query = $"{StandardizedQuery(column)} WHERE {GetSqlFromPredicate(predicate)}";
            if (newConection)
            {
                using var connection = CreateConnection();
                return await connection.QueryFirstOrDefaultAsync<TEntity>(query);
            }
            return await Connection.QueryFirstOrDefaultAsync<TEntity>(query);
        }

        public virtual async Task<int> InsertAsync(TEntity entity, bool newConection = false)
        {
            var query = $"INSERT INTO {TableName} ({GetInsertColumns()}) VALUES ({GetInsertValues()})";
            if (newConection)
            {
                using var connection = CreateConnection();
                return await connection.ExecuteAsync(query, entity);
            }
            return await Connection.ExecuteAsync(query, entity);
        }
        public virtual async Task<int> InsertAsync(List<TEntity> entities, bool newConection = false)
        {
            var query = $"INSERT INTO {TableName} ({GetInsertColumns()}) VALUES ({GetInsertValues()})";
            if (newConection)
            {
                using var connection = CreateConnection();
                return await connection.ExecuteAsync(query, entities);
            }
            return await Connection.ExecuteAsync(query, entities);
        }

        public virtual async Task<Guid> InsertEntityAsync(TEntity entity, bool newConection = false)
        {
            var query = $"INSERT INTO {TableName} ({GetInsertColumns()}) VALUES ({GetInsertValues()}) RETURNING \"Id\"";
            if (newConection)
            {
                using var connection = CreateConnection();
                return await connection.ExecuteScalarAsync<Guid>(query, entity);
            }
            return await Connection.ExecuteScalarAsync<Guid>(query, entity);
        }


        public virtual async Task<bool> UpdateAsync(TEntity entity, string primaryKey = "Id", bool newConection = false)
        {
            int rowsAffected;
            if (newConection)
            {
                using var connection = CreateConnection();
                rowsAffected = await connection.ExecuteAsync(GetUpdateQuery(primaryKey), entity);
            }
            else
            {
                rowsAffected = await Connection.ExecuteAsync(GetUpdateQuery(primaryKey), entity);
            }
            return rowsAffected > 0;
        }

        public virtual async Task<bool> DeleteAsync(Guid id, bool newConection = false)
        {
            int rowsAffected;
            var query = $"DELETE FROM {TableName} WHERE \"Id\" = @Id";
            if (newConection)
            {
                using var connection = CreateConnection();
                rowsAffected = await connection.ExecuteAsync(query, new { Id = id });
            }
            else
            {
                rowsAffected = await Connection.ExecuteAsync(query, new { Id = id });
            }
            return rowsAffected > 0;
        }

        public virtual async Task<bool> SoftDeleteAsync(Guid id, bool newConection = false)
        {
            var query = $"UPDATE {TableName} SET \"IsDelete\" = true WHERE \"Id\" = @Id";
            int rowsAffected;
            if (newConection)
            {
                using var connection = CreateConnection();
                rowsAffected = await connection.ExecuteAsync(query, new { Id = id });
            }
            else
            {
                rowsAffected = await Connection.ExecuteAsync(query, new { Id = id });
            }
            return rowsAffected > 0;
        }

        public virtual async Task<PagedResults<TEntity>> GetByPageAsync(Expression<Func<TEntity, bool>> predicate, int pageNumber = 1, int pageSize = 10, string column = "", bool newConection = false)
        {
            var whereStatement = predicate == null ? "" : $" WHERE {GetSqlFromPredicate(predicate)} ";
            var offset = pageSize * (pageNumber - 1);
            var queries = @$"
                {StandardizedQuery(column)}
                {whereStatement}
                LIMIT @PageSize
                OFFSET @Offet ;";

            queries += $"SELECT COUNT(*) AS TotalItems FROM {TableName} {whereStatement};";

            GridReader multi = null;
            if (newConection)
            {
                using var connection = CreateConnection();
                multi = await connection.QueryMultipleAsync(queries,
                new
                {
                    Offet = offset,
                    PageSize = pageSize
                });
            }
            else
            {
                multi = await Connection.QueryMultipleAsync(queries,
                    new
                    {
                        Offet = offset,
                        PageSize = pageSize
                    });
            }
            var items = await multi.ReadAsync<TEntity>().ConfigureAwait(false);

            var totalItems = await multi.ReadFirstAsync<int>().ConfigureAwait(false);
            multi.Dispose();
            var result = new PagedResults<TEntity>(totalItems, pageNumber, pageSize)
            {
                Items = items
            };
            return result;
        }

        public virtual async Task<IEnumerable<TEntity>> GetByCustomQuery(string query, object param = null, bool newConection = false)
        {
            if (newConection)
            {
                using var connection = CreateConnection();
                return await connection.QueryAsync<TEntity>(query, param);
            }
            return await Connection.QueryAsync<TEntity>(query, param);
        }

        #region Private method

        private static IEnumerable<PropertyInfo> GetInsertColumnsNotMapped()
        {
            var t = typeof(TEntity);
            return t.GetProperties().Where(p => !p.CustomAttributes.Any() || !p.CustomAttributes.Any(q => q.AttributeType.Name == "NotMappedAttribute"));
        }

        private static string GetInsertColumns()
        {
            var properties = GetInsertColumnsNotMapped();
            return "\"" + string.Join("\", \"", properties.Select(p => p.Name)) + "\"";
        }

        private static string GetInsertValues()
        {
            var properties = GetInsertColumnsNotMapped();
            return string.Join(", ", properties.Select(p => "@" + p.Name));
        }

        private string GetUpdateQuery(string primaryKey)
        {
            var properties = GetInsertColumnsNotMapped();
            var setClauses = properties
                               .Where(p => !p.Name.Equals(primaryKey, StringComparison.OrdinalIgnoreCase))
                               .Select(p => $"\"{p.Name}\" = @{p.Name}");
            var setClause = string.Join(", ", setClauses);

            var query = $"UPDATE {TableName} SET {setClause} WHERE \"{primaryKey}\" = @{primaryKey}";
            return query;
        }

        private string GetSqlFromPredicate(Expression<Func<TEntity, bool>> predicate)
        {
            var sql = Visit(predicate.Body);

            return sql;

            string Visit(Expression expression)
            {
                if (expression is BinaryExpression binaryExpression)
                {
                    var left = Visit(binaryExpression.Left);
                    var right = Visit(binaryExpression.Right);
                    if (binaryExpression.NodeType == ExpressionType.AndAlso)
                    {
                        return $"({left} AND {right})";
                    }
                    else if (binaryExpression.NodeType == ExpressionType.OrElse)
                    {
                        return $"({left} OR {right})";
                    }
                    //For check null case
                    else if (binaryExpression.Right is ConstantExpression expressionRight && expressionRight.Value == null
                        && (binaryExpression.NodeType == ExpressionType.Equal || binaryExpression.NodeType == ExpressionType.NotEqual))
                    {
                        if (binaryExpression.NodeType == ExpressionType.Equal)
                        {
                            return $"({left} IS NULL)";
                        }
                        else
                        {
                            return $"({left} IS NOT NULL)";
                        }
                    }
                    else
                    {
                        var comparison = GetComparisonOperator(binaryExpression.NodeType);
                        return $"{left} {comparison} {right}";
                    }
                }
                else if (expression is MemberExpression memberExpression)
                {
                    if (memberExpression.Expression is ConstantExpression constantExpression)
                    {
                        var value = Expression.Lambda(memberExpression).Compile().DynamicInvoke();
                        return $"'{value}'";
                    }
                    else
                    {
                        var propertyName = GetPropertyName(memberExpression);
                        return propertyName;
                    }
                }
                else if (expression is ConstantExpression constantExpression)
                {
                    return $"'{constantExpression.Value}'";
                }
                else if (expression is UnaryExpression unaryExpression)
                {
                    var reduceExpression = unaryExpression.Operand.Reduce();
                    //For bool value
                    if (reduceExpression.Type.Name == nameof(Boolean) && reduceExpression is ConstantExpression constantBoolExpression)
                    {
                        return $"'{constantBoolExpression.Value}'";
                    }
                    //For enumtype(left)
                    else if (reduceExpression.Type.IsEnum)
                    {
                        var value = GetPropertyName(reduceExpression);
                        return $"{value}";
                    }
                    else
                    {
                        throw new NotSupportedException($"Unsupported expression type: {reduceExpression.Type.Name}");
                    }
                }
                else
                {
                    throw new NotSupportedException($"Unsupported expression type: {expression.NodeType}");
                }
            }
            string GetPropertyName(Expression expression)
            {
                var propertyName = Regex().Replace(expression.ToString(), "");
                return $"\"{propertyName}\"";
            }
        }

        private static string GetComparisonOperator(ExpressionType expressionType)
        {
            return expressionType switch
            {
                ExpressionType.Equal => "=",
                ExpressionType.NotEqual => "<>",
                ExpressionType.LessThan => "<",
                ExpressionType.LessThanOrEqual => "<=",
                ExpressionType.GreaterThan => ">",
                ExpressionType.GreaterThanOrEqual => ">=",
                _ => throw new NotSupportedException($"Unsupported binary operator: {expressionType}"),
            };
        }

        private bool IsReservedWord(string word)
        {
            var reservedWords = new List<string>
                {
                    "select", "from", "where", "and", "or",
                };

            return reservedWords.Contains(word.ToLower());
        }

        [GeneratedRegex("^.+\\.")]
        private static partial Regex Regex();

        #endregion Private method
    }
}