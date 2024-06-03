using Mcsg.Function.Job.Services;
using Mcsg.Lib.Common.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Mcsg.Function.Job
{
    public class PaymentTransactionFunction
    {
        private readonly ILogger<PaymentTransactionFunction> _logger;
        private readonly IPaymentService _paymentService;
        public PaymentTransactionFunction(ILogger<PaymentTransactionFunction> logger, IPaymentService paymentService)
        {
            _logger = logger;
            _paymentService = paymentService;
        }

        [Function(nameof(PaymentTransactionFunction))]
        public async Task Run([QueueTrigger("paymenttransqueue", Connection = "Function:AzureBlobStorageConnection")] string data)
        {
            _logger.LogInformation($"C# Queue trigger function processed: {data}");
            var paymentData = JsonConvert.DeserializeObject<PaymentTransData>(data);
            switch (paymentData.Type)
            {
                case PaymentTransType.ZALO_PAY:
                    {
                        await _paymentService.ZPQueryOrderAsync(paymentData);
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
        }
    }
}
