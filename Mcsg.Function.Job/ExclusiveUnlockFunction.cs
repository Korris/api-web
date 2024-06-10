using Mcsg.Function.Job.Services;

namespace Mcsg.Function.Job
{
    public class ExclusiveUnlockFunction
    {
        private readonly ILogger<SmartCountCommentFunction> _logger;
        private readonly IExclusiveUnlockService _exclusiveUnlockService;

        public ExclusiveUnlockFunction(
            ILogger<SmartCountCommentFunction> logger,
            IExclusiveUnlockService exclusiveUnlockService)
        {
            _logger = logger;
            _exclusiveUnlockService = exclusiveUnlockService;
        }

        /*[Function(nameof(ExclusiveUnlockFunction))]
        public async Task Run([TimerTrigger(FunctionConstant.ExclusiveUnlockCron)] TimerInfo myTimer)
        {
            _logger.LogInformation($"ExclusiveUnlockFunction trigger function executed at: {DateTime.Now}");

            if (myTimer.ScheduleStatus is not null)
            {
                _logger.LogInformation($"Next timer ExclusiveUnlockFunction schedule at: {myTimer.ScheduleStatus.Next}");
                await _exclusiveUnlockService.Run();
            }
        }*/
    }
}
