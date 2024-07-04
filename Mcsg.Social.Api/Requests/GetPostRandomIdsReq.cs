namespace Mcsg.Social.Api.Requests
{
    public class GetPostRandomIdsReq
    {
        public int AmountItem { get; set; }
        public List<string>? PostRandomIds { get; set; }
        public bool IsGetAllType { get; set; }
    }
}