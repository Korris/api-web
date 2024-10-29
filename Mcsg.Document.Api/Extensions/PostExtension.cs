namespace Mcsg.Document.Api.Extensions;

using Constants;
using Enums;
using Models;

public static class PostExtension
{
    public static string ToPostSeriesStatus(this PostSeriesSelectedType type)
    {
        return type switch
        {
            PostSeriesSelectedType.HIT => PostConst.PostSeriesStatus.Hit,
            PostSeriesSelectedType.LATEST => PostConst.PostSeriesStatus.Latest,
            PostSeriesSelectedType.COMPLETED => PostConst.PostSeriesStatus.Completed,
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
