using System.ComponentModel.DataAnnotations;

namespace Mcsg.Document.Api.Requests;

using Common.Core.Requests;

public class DocumentRelationPostSeriesR : PaginatedR
{
    [Required]
    public string? HashId { get; set; }
}
