namespace Mcsg.Wallet.Job.Interfaces;

using Domain.Entities;

public interface IEmailService
{
    Task SendEmailAsync(Job job);
}
