using System.ComponentModel.DataAnnotations;

namespace Mcsg.Api.Areas.Document.Requests;

using Common.Core.Requests;

public class DocumentRelationPostSeriesR : PaginatedR
{
    [Required]
    public string? HashId { get; set; }
}
