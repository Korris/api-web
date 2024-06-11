namespace Mcsg.Function.Job.Interfaces;

using Lib.Common.Models;

public interface IPaymentService
{
    Task ZPQueryOrderAsync(PaymentTransData data);
}
