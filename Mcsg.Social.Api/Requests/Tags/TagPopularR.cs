using System.ComponentModel;

namespace Mcsg.Social.Api.Requests;

using Common.Core.Enums;
using Lib.Common.Models;

public class TagPopularR : PaginatedR
{
    [DefaultValue(null)]
    public PostType? PostType { get; set; }
}
