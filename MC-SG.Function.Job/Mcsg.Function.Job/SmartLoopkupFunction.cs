using Dapper;
using Mcsg.Lib.Common.Models;
using Mcsg.Lib.Data.Domain.Entities;
using Mcsg.Lib.Data.Enums;
using Mcsg.Lib.Data.Repositories;
using Mcsg.Lib.Data.Repositories.Interface;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Entities = Mcsg.Lib.Data.Domain.Entities;

namespace Mcsg.Function.Job
{
    public class SmartLookupFunction
    {
        private readonly ILogger<SmartLookupFunction> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Entities.SmartLookup> _smartLookupRepository;
        private readonly IRepository<Entities.Post> _postRepository;
        private readonly IRepository<Entities.TagPost> _tagPostRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Entities.Tag> _tagRepository;

        public SmartLookupFunction(
            ILogger<SmartLookupFunction> logger,
            IUnitOfWork unitOfWork,
            IRepository<SmartLookup> smartLookupRepository,
            IRepository<Post> postRepository,
            IRepository<User> userRepository,
            IRepository<Tag> tagRepository,
            IRepository<Entities.TagPost> tagPostRepository)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _smartLookupRepository = smartLookupRepository;
            _postRepository = postRepository;
            _userRepository = userRepository;
            _tagRepository = tagRepository;
            _tagPostRepository = tagPostRepository;
        }

        [Function(nameof(SmartLookupFunction))]
        public async Task Run([QueueTrigger("smartlookupqueue", Connection = "Function:AzureBlobStorageConnection")] string data)
        {
            string logMessage = $"C# Queue trigger function processed: {data}";
            _logger.LogInformation(logMessage);

            var smartLookupData = JsonConvert.DeserializeObject<SmartLookupData>(data);

            switch (smartLookupData.KeywordType)
            {
                case LookupKeywordType.People:
                    await _smartLookupRepository.Connection.ExecuteAsync(UpdateSmartLookupPeopleCommand, new { Name = smartLookupData.ProfileName, KeywordType = (int)smartLookupData.KeywordType });
                    break;
                case LookupKeywordType.Tag:
                    if (smartLookupData.Tags.Any())
                    {
                        foreach (var tag in smartLookupData.Tags)
                        {
                            var countTagPost = await _smartLookupRepository.Connection
                                .QueryFirstOrDefaultAsync<long>(CountTagPostCommand, new { tag });

                            await _smartLookupRepository.Connection.ExecuteAsync(UpdateSmartLookupTagCommand, new
                            {
                                value = countTagPost,
                                tag,
                                KeywordType = (int)smartLookupData.KeywordType
                            });
                        }
                    }
                    break;
            }
        }

        private string UpdateSmartLookupPeopleCommand
        {
            get
            {
                return string.Format(@"UPDATE {0} AS s
                                    SET ""CountCriteria"" = c.count_value
                                    FROM (
                                        SELECT u.""ProfileName"", COUNT(p.""Id"") AS count_value
                                        FROM {1} p
                                        INNER JOIN {2} u ON p.""UserId"" = u.""Id""
                                        WHERE u.""ProfileName"" = @Name AND p.""IsDelete"" = false
                                        GROUP BY u.""ProfileName""
                                    ) AS c
                                    WHERE s.""Keyword"" = c.""ProfileName""
                                    AND s.""Keyword"" = @Name
                                    AND s.""KeywordType"" = @KeywordType;
                                ", _smartLookupRepository.TableName, _postRepository.TableName, _userRepository.TableName);
            }
        }

        private string UpdateSmartLookupTagCommand
        {
            get
            {
                return string.Format(@"UPDATE {0} AS s
                                        SET ""CountCriteria"" = @value                             
                                        WHERE s.""Keyword"" = @tag
                                        AND s.""KeywordType"" = @KeywordType;"
                    , _smartLookupRepository.TableName);
            }
        }

        private string CountTagPostCommand
        {
            get
            {
                return @$"SELECT COUNT(tagpost.""PostId"") AS CountValue
                        FROM {_tagRepository.TableName} tag 
                        INNER JOIN {_tagPostRepository.TableName} tagpost ON tagpost.""TagId"" = tag.""Id""
                        WHERE tag.""Name"" = @tag AND tagpost.""IsDelete"" = false";
            }
        }
    }
}
