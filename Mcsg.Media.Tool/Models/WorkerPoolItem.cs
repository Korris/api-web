namespace Mcsg.Media.Tool.Models
{
    internal class WorkerPoolItem
    {
        public Guid Id { get; set; }
        public Task Task { get; set; }
        public bool IsRunning { get; set; } = true;
    }
}
