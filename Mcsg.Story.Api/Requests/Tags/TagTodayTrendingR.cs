using System.ComponentModel;

namespace Mcsg.Story.Api.Requests;

using Common.Core.Enums;
using Common.Core.Requests;

public class TagTodayTrendingR : PaginatedR
{
    [DefaultValue(null)]
    public PostType? PostType { get; set; }
    [DefaultValue(6)]
    public new int PageSize { get; set; }
}
