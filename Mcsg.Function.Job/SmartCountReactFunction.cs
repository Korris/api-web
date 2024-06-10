using Mcsg.Function.Job.Services;
using Mcsg.Lib.Data.Domain.Entities;
using Mcsg.Lib.Data.Repositories;
using Mcsg.Lib.Data.Repositories.Interface;
using Entities = Mcsg.Lib.Data.Domain.Entities;

namespace Mcsg.Function.Job
{
    public class SmartCountReactFunction
    {
        private readonly ILogger<SmartCountReactFunction> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Entities.SmartCountAction> _smartCountActionRepository;
        private readonly IRepository<Entities.PostReaction> _postReactionRepository;
        private readonly IRepository<Entities.SubPostReaction> _subPostReactionRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Entities.Tag> _tagRepository;
        private readonly ICountService<PostReaction, SubPostReaction> _reactionCountService;

        public SmartCountReactFunction(
            ILogger<SmartCountReactFunction> logger,
            ICountService<PostReaction, SubPostReaction> reactionCountService,
            IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _reactionCountService = reactionCountService;
            _smartCountActionRepository = _unitOfWork.GetRepository<SmartCountAction>();
            _postReactionRepository = _unitOfWork.GetRepository<PostReaction>();
            _subPostReactionRepository = _unitOfWork.GetRepository<SubPostReaction>();
        }

        /*[Function(nameof(SmartCountReactFunction))]
        public async Task Run([QueueTrigger("postreactqueue", Connection = "Function:AzureBlobStorageConnection")] string data)
        {
            string logMessage = $"C# Queue trigger function processed: {data}";
            _logger.LogInformation(logMessage);

            var smartLookupData = JsonConvert.DeserializeObject<SmartCountEntityData>(data);

            await _reactionCountService.RunQueue(smartLookupData);
        }*/
    }
}
