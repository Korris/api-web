using Mcsg.Lib.Data.Domain.Entities;

namespace Mcsg.Media.Tool.Workers
{
    internal interface IWorker
    {
        void CleanupPool();

        bool HasAvailableSlot();

        void Execute(Job jobInfo);
    }
}
