using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Common.Domain.Entities;

using SeedWork;
using SeedWork.Constants;

public partial class Tag : AuditableEntity
{
    [StringLength(Validator.Title.Max)]
    public string? Title { get; set; }

    [StringLength(Validator.Name.Max)]
    public string? Name { get; set; }

    public Guid? AuthorId { get; set; }

    [ForeignKey("AuthorId")]
    [InverseProperty("Tags")]
    public virtual User? Author { get; set; }

    [InverseProperty("Tag")]
    public virtual ICollection<ComicTagPost> ComicTagPosts { get; set; } = new List<ComicTagPost>();

    [InverseProperty("Tag")]
    public virtual ICollection<DocumentTagPost> DocumentTagPosts { get; set; } = new List<DocumentTagPost>();

    [InverseProperty("Tag")]
    public virtual ICollection<SocialTagPost> SocialTagPosts { get; set; } = new List<SocialTagPost>();

    [InverseProperty("Tag")]
    public virtual ICollection<StoryTagPost> StoryTagPosts { get; set; } = new List<StoryTagPost>();

    [InverseProperty("Tag")]
    public virtual ICollection<TagFavorite> TagFavorites { get; set; } = new List<TagFavorite>();
}