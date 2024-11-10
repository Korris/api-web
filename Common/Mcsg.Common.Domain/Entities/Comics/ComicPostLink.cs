using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Common.Domain.Entities;

public partial class ComicPostLink : BasePostLink
{
    [ForeignKey("PostId")]
    [InverseProperty("ComicPostLinks")]
    public virtual ComicPost Post { get; set; } = null!;
}
