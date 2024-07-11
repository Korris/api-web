using System.ComponentModel;

namespace Mcsg.Social.Api.Requests;

using Common.Core.Enums;
using Common.Core.Requests;

public class TagPopularR : PaginatedR
{
    [DefaultValue(null)]
    public PostType? PostType { get; set; }
}
