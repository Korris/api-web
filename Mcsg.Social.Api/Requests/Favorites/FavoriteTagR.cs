using System.ComponentModel;

namespace Mcsg.Social.Api.Requests;

using Lib.Common.Models;

public class FavoriteTagR : PaginatedR
{
    [DefaultValue(6)]
    public new int PageSize { get; set; }
}
