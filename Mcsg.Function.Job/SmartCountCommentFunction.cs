using Mcsg.Function.Job.Services;
using Mcsg.Lib.Common.Models;
using Mcsg.Lib.Data.Domain.Entities;
using Mcsg.Lib.Data.Repositories;
using Mcsg.Lib.Data.Repositories.Interface;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Entities = Mcsg.Lib.Data.Domain.Entities;

namespace Mcsg.Function.Job
{
    public class SmartCountCommentFunction
    {
        private readonly ILogger<SmartCountCommentFunction> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Entities.SmartCountAction> _smartCountActionRepository;
        private readonly IRepository<Entities.PostComment> _postCommentRepository;
        private readonly IRepository<Entities.SubPostComment> _subPostCommentRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Entities.Tag> _tagRepository;
        private readonly ICountService<PostComment, SubPostComment> _commentCountService;

        public SmartCountCommentFunction(
            ILogger<SmartCountCommentFunction> logger,
            ICountService<PostComment, SubPostComment> commentCountService,
            IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _postCommentRepository = _unitOfWork.GetRepository<Entities.PostComment>();
            _subPostCommentRepository = _unitOfWork.GetRepository<Entities.SubPostComment>();
            _smartCountActionRepository = _unitOfWork.GetRepository<Entities.SmartCountAction>();
            _commentCountService = commentCountService;
        }

        [Function(nameof(SmartCountCommentFunction))]
        public async Task Run([QueueTrigger("postcommentqueue", Connection = "Function:AzureBlobStorageConnection")] string data)
        {
            string logMessage = $"C# Queue trigger function processed: {data}";
            _logger.LogInformation(logMessage);

            var smartLookupData = JsonConvert.DeserializeObject<SmartCountEntityData>(data);

            await _commentCountService.RunQueue(smartLookupData);
        }

    }
}
