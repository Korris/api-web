namespace Mcsg.Function.Job.Interfaces;

using Common.Domain.Entities;

public interface IEmailService
{
    Task SendEmailAsync(Job job);
}
