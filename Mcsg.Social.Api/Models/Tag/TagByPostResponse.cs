namespace Mcsg.Social.Api.Models.Tag;

public class TagByPostResponse
{
    public string PostHashId { get; set; }
    public Guid PostId { get; set; }
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Name { get; set; }
}
