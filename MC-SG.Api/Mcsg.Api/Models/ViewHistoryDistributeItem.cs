using Mcsg.Lib.Common.Distributor;
using Mcsg.Lib.Common.Models;

namespace Mcsg.Api.Models
{
    public class ViewHistoryDistributeItem : DistributedItem
    {
        public ViewHistoryData Data { get; set; }
    }
}
