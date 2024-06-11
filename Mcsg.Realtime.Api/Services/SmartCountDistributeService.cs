namespace Mcsg.Realtime.Api.Services
{
    using Api.Interfaces;
    using Common.Core.Dtos;
    using Common.Core.Extensions;
    using Lib.Common.Distributor;
    using Models;

    public class SmartCountDistributeService : BaseDistributor
    {
        public SmartCountDistributeService(IServiceProvider serviceProvider)
        {
            _setting = serviceProvider.GetRequiredService<ISetting>();
        }

        public override Task<bool> IsAcceptable(DistributedItem item)
        {
            var isAcceptable = item.GetType() == typeof(SmartCountDistributeItem);
            return Task.FromResult(isAcceptable);
        }

        public override async Task ApplyAction(DistributedItem item)
        {
            var distributeItem = item as SmartCountDistributeItem;
            var msg = new QueueMessageDto(distributeItem.Data);
            _setting.SendMessageToQueue(_setting.NotificationExchange, _setting.NotificationQueuePostComment, msg);
        }

        #region -- Fields --

        /// <summary>
        /// Setting
        /// </summary>
        private readonly ISetting _setting;

        #endregion
    }
}
