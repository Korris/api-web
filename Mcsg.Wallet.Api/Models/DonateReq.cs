namespace Mcsg.Wallet.Api.Models
{
    public class DonateReq
    {
        public Guid ToUserId { get; set; }
        public float Amount { get; set; }
        public string Content { get; set; }
    }
}
