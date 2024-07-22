namespace Mcsg.Function.Job.Interfaces;

using Common.Domain.Entities;

public interface ISmsService
{
    Task SendSmsAsync(Job job);
}
