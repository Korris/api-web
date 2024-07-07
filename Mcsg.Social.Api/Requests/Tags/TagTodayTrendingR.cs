using System.ComponentModel;

namespace Mcsg.Social.Api.Requests;

using Common.Core.Enums;
using Lib.Common.Models;

public class TagTodayTrendingR : PaginatedR
{
    [DefaultValue(null)]
    public PostType? PostType { get; set; }
    [DefaultValue(6)]
    public new int PageSize { get; set; }
}
