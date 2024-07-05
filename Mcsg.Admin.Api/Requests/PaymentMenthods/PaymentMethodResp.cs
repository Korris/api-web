using Mcsg.Lib.Data.Wallet.Enums;
using System.ComponentModel.DataAnnotations;

namespace Mcsg.Admin.Api.DTOs.PaymentMenthods
{
    public class PaymentMethodResp
    {
        public Guid Id { get; set; }
        public PaymentMethodType Type { get; set; }
        [MaxLength(255)]
        public string Name { get; set; }
        [MaxLength(255)]
        public string Logo { get; set; }
        public bool AllowDeposit { get; set; }
        public bool AllowWithdrawal { get; set; }
        public int SelfId { get; set; }
        public string Code { get; set; }
        public string Bin { get; set; }
        public string ShortName { get; set; }
        public string SwiftCode { get; set; }
    }
}
