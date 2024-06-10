using Mcsg.Function.Job.Services;
using Mcsg.Lib.Data.Domain.Entities;
using Mcsg.Lib.Data.Repositories;
using Mcsg.Lib.Data.Repositories.Interface;
using Entities = Mcsg.Lib.Data.Domain.Entities;

namespace Mcsg.Function.Job
{
    public class ViewHistoryFunction
    {
        private readonly ILogger<SmartCountCommentFunction> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Entities.SmartCountAction> _smartCountActionRepository;
        private readonly IRepository<Entities.ViewHistory> _viewHistoryRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Entities.Tag> _tagRepository;
        private readonly ICountService<ViewHistory, ViewHistory> _viewHistoryCountService;

        public ViewHistoryFunction(
            ILogger<SmartCountCommentFunction> logger,
            ICountService<ViewHistory, ViewHistory> viewHistoryCountService,
            IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _viewHistoryRepository = _unitOfWork.GetRepository<Entities.ViewHistory>();
            _smartCountActionRepository = _unitOfWork.GetRepository<Entities.SmartCountAction>();
            _viewHistoryCountService = viewHistoryCountService;
        }

        /*[Function(nameof(ViewHistoryFunction))]
        public async Task Run([QueueTrigger("viewhistoryqueue", Connection = "Function:AzureBlobStorageConnection")] string data)
        {
            string logMessage = $"C# Queue trigger function processed: {data}";
            _logger.LogInformation(logMessage);

            var viewHistory = JsonConvert.DeserializeObject<ViewHistoryData>(data);

            //Check user
            try
            {
                var check = await _viewHistoryRepository.Connection.QueryFirstOrDefaultAsync<Guid?>(GetLastViewFromUser, new
                {
                    EntityId = viewHistory.EntityId,
                    userid = viewHistory.UserId,
                    EntityType = viewHistory.EntityType,
                    IpAddress = viewHistory.IdAddress
                });

                if (check != null)
                {
                    //Out
                    return;
                }
            }
            catch (Exception e)
            {

                throw;
            }


            //Do save view history
            ViewHistory history = new ViewHistory
            {
                EntityType = viewHistory.EntityType,
                EntityId = viewHistory.EntityId,
                CreatedDate = DateTime.UtcNow,
                IpAddress = viewHistory.IdAddress,
                SubType = viewHistory.SubType,
                UsedId = viewHistory.UserId
            };
            await _viewHistoryRepository.InsertAsync(history);

            var smartLookupData = new SmartCountEntityData
            {
                ActionType = ActionType.VIEW,
                EntityId = viewHistory.EntityId,
                EntityType = viewHistory.EntityType,
                SubType = viewHistory.SubType,
                IsRemove = false,
            };
            await _viewHistoryCountService.RunQueue(smartLookupData);

        }*/

        private string GetLastViewFromUser
        {
            get
            {
                return @"SELECT ""Id""
	            FROM ""ViewHistories""
	            WHERE ""EntityId"" = @EntityId AND ""UsedId"" = @userid	
	            AND ""EntityType"" = @EntityType AND ""IpAddress"" = @IpAddress ;";
            }
        }

    }
}
