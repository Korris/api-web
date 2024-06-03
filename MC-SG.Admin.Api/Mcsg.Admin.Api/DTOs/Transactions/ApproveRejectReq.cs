namespace Mcsg.Admin.Api.DTOs.Transactions
{
    public class ApproveReq
    {
        public Guid? TransactionId { get; set; }
    }
    public class RejectReq
    {
        public Guid? TransactionId { get; set; }
        public string Reason { get; set; }
    }
    public class ApproveRefReq
    {
        public string? ReferenceNumber { get; set; }
    }
    public class RejectRefReq
    {
        public string? ReferenceNumber { get; set; }
        public string Reason { get; set; }
    }
}
