using Mcsg.Lib.Data.Domain.Entities;

namespace Mcsg.Media.Tool.Features
{
    internal interface IWorker
    {
        void CleanupPool();

        bool HasAvailableSlot();

        void Execute(Job jobInfo);
    }
}