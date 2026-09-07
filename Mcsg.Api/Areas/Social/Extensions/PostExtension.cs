using System.Text.RegularExpressions;

namespace Mcsg.Api.Areas.Social.Extensions;

using Common.Core.Enums;
using Mcsg.Api.Areas.Social.Constants;
using Mcsg.Api.Areas.Social.Models;

public static class PostExtension
{
    public static string ToPostSeriesStatus(this PostSeriesSelectedType type)
    {
        return type switch
        {
            PostSeriesSelectedType.Hit => PostConst.PostSeriesStatus.Hit,
            PostSeriesSelectedType.Latest => PostConst.PostSeriesStatus.Latest,
            PostSeriesSelectedType.Completed => PostConst.PostSeriesStatus.Completed,
            _ => throw new NotSupportedException($"Unsupported entity type: {type}"),
        };
    }

    public static string ToSeriesStatus(this PostSeriesResponse model)
    {
        return model.IsCompleted switch
        {
            false => PostConst.PostSeriesStatus.Latest,
            true => PostConst.PostSeriesStatus.Completed,
            _ => PostConst.PostSeriesStatus.Latest,
        };
    }

    public static List<string> ExtractHashtags(this string content)
    {
        var result = new List<string>();
        if (string.IsNullOrEmpty(content))
        {
            return result;
        }

        // Use regular expression to find all hashtags
        Regex regex = new Regex(@"#(\w+)");
        MatchCollection matches = regex.Matches(content);

        List<string> hashtags = new List<string>();
        foreach (Match match in matches)
        {
            result.Add(match.Groups[1].Value);
        }

        return result;
    }
}
