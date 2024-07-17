using System.ComponentModel;

namespace Mcsg.Comic.Api.Requests;

public class ComicTopPostRecommendedR : ComicTopPostR
{
    [DefaultValue(3)]
    public int PageSize { get; set; }
}
