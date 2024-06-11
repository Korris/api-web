namespace Mcsg.Function.Job.Interfaces;

using Entities = Lib.Data.Domain.Entities;

public interface IEmailService
{
    Task SendEmailAsync(Entities.Job job);
}
