using Dapper;
using Npgsql;
using System.Collections.Concurrent;

namespace Mcsg.Media.Tool
{
    using Common.Core.Enums;
    using Lib.Data.Domain.Entities;
    using Lib.Data.Enums;
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
            var query = @"SELECT ""Id"", ""Status"",""Data"", ""JobType"" FROM ""Jobs""
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
            var command = @"UPDATE ""Jobs"" SET ""Status"" = @status, ""Error"" = @error WHERE ""Id"" = @id ";

            using var conn = new NpgsqlConnection(_connectionString);
            await conn.ExecuteAsync(command,
                new
                {
                    id = jobId,
                    status = jobStatus.GetHashCode(),
                    error
                });
        }

        public async Task UpdateResourceStatus(Guid resourceId, ResourceStatus resourceStatus, string url, string shareUrl)
        {
            var command = @"UPDATE ""Resources""
                            SET ""Status""= @status,
                                 ""Url"" = @url,
                                ""ShareUrl"" = @shareUrl
                            WHERE ""Id"" = @id ";

            using var conn = new NpgsqlConnection(_connectionString);
            await conn.ExecuteAsync(command, new
            {
                status = resourceStatus.GetHashCode(),
                url,
                shareUrl = shareUrl,
                id = resourceId,
            });
        }

        public async Task<VideoNotificationModel> LoadResource(string hashId)
        {
            var query = @"SELECT res.""Id"", res.""HashId""
	                    , res.""AuthorId"", (CASE WHEN us.""ProfileName"" IS NULL THEN us.""UserName""  ELSE us.""ProfileName"" END) AS AuthorName
	                    , sub.""PostId"", post.""HashId"" AS ""PostHashId"" 
	                    FROM public.""Resources"" res
	                    LEFT JOIN identity.""Users"" us ON res.""AuthorId"" = us.""Id""
	                    LEFT JOIN public.""SubPosts"" sub ON res.""SubPostId"" = sub.""Id""
	                    LEFT JOIN public.""Posts"" post ON sub.""PostId"" = post.""Id""
	                    WHERE res.""HashId"" = @HashId";

            using var conn = new NpgsqlConnection(_connectionString);
            var resource = await conn.QueryFirstOrDefaultAsync<VideoNotificationModel>(query,
                new
                {
                    HashId = hashId
                });

            return resource;
        }
    }
}
