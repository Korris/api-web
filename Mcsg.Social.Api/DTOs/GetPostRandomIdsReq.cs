namespace Mcsg.Social.Api.DTOs
{
    public class GetPostRandomIdsReq
    {
        public int NumOfItem { get; set; }
        public List<Guid> PostRandomIds { get; set; }
    }
}