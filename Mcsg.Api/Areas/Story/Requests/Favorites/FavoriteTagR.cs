using System.ComponentModel;

namespace Mcsg.Api.Areas.Story.Requests;

using Common.Core.Requests;

public class FavoriteTagR : PaginatedR
{
    [DefaultValue(6)]
    public new int PageSize { get; set; }
}
