namespace Mcsg.Wallet.Api.Models;

using Domain.Enums;

public class DepositResp
{
    public string QRCodeUrl { get; set; }
    public Guid TransactionId { get; set; }
    public string ToBankName { get; set; }
    public string ToAccountName { get; set; }
    public string ToAccountNumber { get; set; }
    public TransactionType Type { get; set; }
    public string Target { get; set; }
    public string GatewayUrl { get; set; }
}
