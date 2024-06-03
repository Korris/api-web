namespace Mcsg.Analytic.Api.Models.Response
{
    public class PostView
    {
        public Guid Id { get; set; }
        public int CountView { get; set; }
        public List<SubPostView> SubPost { get; set; }
    }
}
