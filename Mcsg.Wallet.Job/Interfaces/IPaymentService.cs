namespace Mcsg.Wallet.Job.Interfaces;

using Lib.Common.Models;

public interface IPaymentService
{
    Task ZPQueryOrderAsync(PaymentTransData data);
}
