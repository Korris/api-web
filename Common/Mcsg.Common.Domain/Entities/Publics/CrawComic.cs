using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Common.Domain.Entities;

using SeedWork;
using Core.Enums;

[Table("CrawComics")]
public class CrawComic : AuditableEntity
{
    public string? Name { get; set; }
    public string? Url { get; set; }
    public string? Author { get; set; }
    public string? Status { get; set; }
    public string? Category { get; set; }
    public string? Description { get; set; }
    public string? View { get; set; }
    public string? Comment { get; set; }
    public string? Follow { get; set; }
    public string? Rating { get; set; }
    public int TotalChapter { get; set; }
    public string? Avatar { get; set; }
    public string? ExternalCode { get; set; }
    public ComicExternalResource ExternalResource { get; set; }
    public string? ExternalLastedUpdate { get; set; }
    public CrawStatus CrawStatus { get; set; }
}
