using System.ComponentModel;

namespace Mcsg.Document.Api.Requests;

public class DocumentTopPostRecommendedR : DocumentTopPostR
{
    [DefaultValue(3)]
    public int PageSize { get; set; }
}
