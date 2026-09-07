using System.Text.Json.Serialization;

namespace Mcsg.Api.Areas.Story.Dtos;

using Common.Core.Enums;
using Common.SeedWork.Converters;

public class SubPostBasic
{
    public Guid Id { get; set; }
    public string HashId { get; set; }
    public string Title { get; set; }
    public string Name { get; set; }
    public PostPermission Permission { get; set; }
    public PostStatus Status { get; set; }

    [JsonConverter(typeof(IsoDateTimeConverter))]
    public DateTime CreatedOn { get; set; }

    public DateTime? PublishDate { get; set; }
    public Guid? CreatedBy { get; set; }
    public Guid? UserId { get; set; }
    public string CreatorNote { get; set; }
    public bool IsExclusive { get; set; }
    public bool IsArchived => Status == PostStatus.Inactive;
    public bool IsCensored { get; set; }
}
