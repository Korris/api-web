namespace Mcsg.Function.Job.Interfaces;

using Entities = Lib.Data.Domain.Entities;

public interface ISmsService
{
    Task SendSmsAsync(Entities.Job job);
}
