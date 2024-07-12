namespace Mcsg.Lib.Common.Extensions;

public static class IEnumerableExtensions
{
    public static IEnumerable<T> DistinctBy<T, TKey>(this IEnumerable<T> items, Func<T, TKey> keySelector)
    {
        var seenKeys = new HashSet<TKey>();
        foreach (var item in items)
        {
            if (seenKeys.Add(keySelector(item)))
            {
                yield return item;
            }
        }
    }
}
