namespace Mcsg.Story.Api.Extensions;

using Common.Core.Enums;
using Constants;
using Models;

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
}
