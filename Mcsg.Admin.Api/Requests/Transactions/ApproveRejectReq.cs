namespace Mcsg.Admin.Api.Requests
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
