namespace Mcsg.Api.Areas.Story.Requests;

using Common.Core.Enums;
using Common.Core.Requests;

/// <summary>
/// FormBase request
/// </summary>
public class StorySubPostFormBaseR : IdBaseR
{
    #region -- Properties --

    /// <summary>
    /// PostHashId
    /// </summary>
    public string? PostHashId { get; set; }

    public string? Title { get; set; }
    public string? Name { get; set; }

    public string? Body { get; set; }

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
