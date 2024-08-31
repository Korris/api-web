namespace Mcsg.Comic.Api.Requests;

using Common.Core.Enums;
using Common.Core.Requests;
using Common.Domain.Dtos;

/// <summary>
/// FormBase request
/// </summary>
public class ComicSubPostFormBaseR : IdBaseR
{
    public string? Title { get; set; }
    public string? Name { get; set; }

    public List<ResourcePostDto> Files { get; set; }

    #region Setting
    public bool IsPublicNow { get; set; }
    public PostPermission Permission { get; set; }
    public PostStatus Status { get; set; }
    public DateTime? PublishDate { get; set; }
    public DateTime? PublishDateUtc => PublishDate == null ? DateTime.UtcNow : PublishDate.Value.AddMinutes(TimezoneOffset);
    public bool IsEnableComment { get; set; }
    public bool IsPremium { get; set; }
    public bool IsAutoGenerateOrder { get; set; }
    public float? Order { get; set; }
    #endregion
}
