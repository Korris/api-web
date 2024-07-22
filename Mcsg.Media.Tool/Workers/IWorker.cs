namespace Mcsg.Media.Tool.Workers
{
    using Common.Domain.Entities;

    internal interface IWorker
    {
        void CleanupPool();

        bool HasAvailableSlot();

        void Execute(Job jobInfo);
    }
}
