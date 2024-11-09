namespace Mcsg.Wallet.Job.Interfaces;

using Common.Models;

public interface IPaymentService
{
    Task ZPQueryOrderAsync(PaymentTransData data);
}
