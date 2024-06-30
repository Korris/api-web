namespace Mcsg.Social.Api.Requests
{
    public class GetPostRandomIdsReq
    {
        public int NumOfItem { get; set; }
        public List<Guid> PostRandomIds { get; set; }
    }
}