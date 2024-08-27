using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Common.Domain.Entities;

using Core.Enums;
using SeedWork;
using SeedWork.Constants;

public class BaseResource : AuditableEntity
{
    [StringLength(Validator.Title.Max)]
    public string? Title { get; set; }

    public Guid? AuthorId { get; set; }
    public string? HashId { get; set; }
    public Guid? SubPostId { get; set; }
    public string? Name { get; set; }
    public string? Url { get; set; }
    public string? BucketName { get; set; }
    public int Order { get; set; }
    public double Size { get; set; } // bytes
    public int Width { get; set; }
    public int Height { get; set; }
    public ResourceType Type { get; set; }
    public ResourceLocationType LocationType { get; set; }
    public ResourceStatus Status { get; set; } = ResourceStatus.Done;

    /// <summary>
    /// MicroService
    /// </summary>
    [NotMapped]
    public string MicroService { get; set; } = Core.Enums.MicroService.Social.ToString();
}