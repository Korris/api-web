using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Common.Domain.Entities;

public partial class ComicResource : BaseResource
{
    [ForeignKey("AuthorId")]
    [InverseProperty("ComicResources")]
    public virtual User? Author { get; set; }

    [InverseProperty("Resource")]
    public virtual ICollection<ComicPostComment> ComicPostComments { get; set; } = new List<ComicPostComment>();

    [InverseProperty("Resource")]
    public virtual ICollection<ComicSubPostComment> ComicSubPostComments { get; set; } = new List<ComicSubPostComment>();

    [ForeignKey("SubPostId")]
    [InverseProperty("ComicResources")]
    public virtual ComicSubPost? SubPost { get; set; }
}
