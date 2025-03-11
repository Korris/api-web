using Dapper;
using Npgsql;
using System.Collections.Concurrent;

namespace Mcsg.Media.Tool;

using Common.Core.Enums;
using Common.Domain.Entities;
using Models;

internal class DbService
{
    private readonly string _connectionString;

    public DbService(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task LoadActiveJobs(ConcurrentQueue<Job> currentQueue)
    {
        var query = @"SELECT ""Id"", ""Status"",""Data"", ""JobType"" FROM system.""Jobs""
                         WHERE ""JobCategory"" = @jobCategory AND ""Status"" = @jobStatus";

        using var conn = new NpgsqlConnection(_connectionString);
        var jobs = await conn.QueryAsync<Job>(query,
            new
            {
                jobCategory = JobCategory.Media.GetHashCode(),
                jobStatus = JobStatus.Queued.GetHashCode()
            });

        foreach (var item in jobs)
        {
            if (!currentQueue.Any(x => x.Id == item.Id))
            {
                await UpdateJobStatus(item.Id, JobStatus.Processing, string.Empty);
                currentQueue.Enqueue(item);
            }
        }
    }

    public async Task UpdateJobStatus(Guid jobId, JobStatus jobStatus, string error)
    {
        var command = @"UPDATE system.""Jobs"" SET ""Status"" = @status, ""Error"" = @error WHERE ""Id"" = @id ";

        using var conn = new NpgsqlConnection(_connectionString);
        await conn.ExecuteAsync(command,
            new
            {
                id = jobId,
                status = jobStatus.GetHashCode(),
                error
            });
    }

    public async Task<ResourceCountModel> GetResourceCount(Guid resourceId)
    {
        var command = @"SELECT 
                        COUNT(sr.*) AS TotalResourcesInSamePost,
                        COUNT(CASE WHEN sr.""CompressedSize"" > 0 THEN 1 END)
                FROM 
                    social.""SocialResources"" sr
                INNER JOIN 
                    social.""SocialSubPosts"" sp ON sr.""SubPostId"" = sp.""Id""
                INNER JOIN 
                    social.""SocialPosts"" sc ON sp.""PostId"" = sc.""Id""
                WHERE 
                    sc.""Id"" = (
                        SELECT sc2.""Id""
                        FROM social.""SocialResources"" sr2
                        INNER JOIN social.""SocialSubPosts"" sp2 ON sr2.""SubPostId"" = sp2.""Id""
                        INNER JOIN social.""SocialPosts"" sc2 ON sp2.""PostId"" = sc2.""Id""
                        WHERE sr2.""Id"" = @resourceId
                    )";

        using var conn = new NpgsqlConnection(_connectionString);

        return await conn.QueryFirstAsync<ResourceCountModel>(command, new { resourceId });
    }

    public async Task UpdateResourceStatus(Guid resourceId, ResourceStatus resourceStatus, string url, string bucketName, MicroService microService, long compressedSize)
    {
        var command = @"UPDATE {0}.""{1}Resources""
                            SET ""Status""= @status,
                                 ""Url"" = @url,
                                ""BucketName"" = @bucketName,
                                ""CompressedSize"" = @compressedSize
                            WHERE ""Id"" = @id ";

        if (microService != MicroService.Comic && microService != MicroService.Story)
        {
            microService = MicroService.Social;
        }
        var prefix = microService.ToString();
        var schema = prefix.ToLower();
        command = string.Format(command, schema, prefix);

        using var conn = new NpgsqlConnection(_connectionString);
        await conn.ExecuteAsync(command, new
        {
            status = resourceStatus.GetHashCode(),
            url,
            bucketName,
            id = resourceId,
            compressedSize
        });
    }

    public async Task<VideoNotificationModel?> LoadResource(string hashId, MicroService microService)
    {
        var query = @"SELECT res.""Id"", res.""HashId""
                        , res.""AuthorId"", (CASE WHEN us.""ProfileName"" IS NULL THEN us.""UserName""  ELSE us.""ProfileName"" END) AS AuthorName
                        , sub.""PostId"", post.""HashId"" AS ""PostHashId"" 
                        FROM {0}.""{1}Resources"" res
                        LEFT JOIN identity.""Users"" us ON res.""AuthorId"" = us.""Id""
                        LEFT JOIN {0}.""{1}SubPosts"" sub ON res.""SubPostId"" = sub.""Id""
                        LEFT JOIN {0}.""{1}Posts"" post ON sub.""PostId"" = post.""Id""
                        WHERE res.""HashId"" = @HashId";

        if (microService != MicroService.Comic && microService != MicroService.Story)
        {
            microService = MicroService.Social;
        }
        var prefix = microService.ToString();
        var schema = prefix.ToLower();
        query = string.Format(query, schema, prefix);

        using var conn = new NpgsqlConnection(_connectionString);
        var resource = await conn.QueryFirstOrDefaultAsync<VideoNotificationModel>(query,
            new
            {
                HashId = hashId
            });

        return resource;
    }
}
