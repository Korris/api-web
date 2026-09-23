using System.Text.RegularExpressions;

namespace Mcsg.Api.Validators;

using Common.SeedWork.Constants;

/// <summary>
/// Shared FluentValidation predicates for a "tags" array (Game / TapShow post forms), same rules as ComicPostFormBaseV
/// </summary>
public static class HashtagRules
{
    /// <summary>
    /// One tag: 1..33 chars after an optional leading '#', letters / digits / underscore only
    /// </summary>
    public static bool Valid(string? tag)
    {
        var name = (tag ?? string.Empty).Trim().TrimStart('#');
        return name.Length >= Validator.Hashtag.Min &&
               name.Length <= Validator.Hashtag.Max &&
               Regex.IsMatch(name, Regular.Tag);
    }

    /// <summary>
    /// No duplicates once normalized (case-insensitive, '#' ignored)
    /// </summary>
    public static bool NoDuplicate(List<string>? tags)
    {
        if (tags == null || tags.Count == 0)
        {
            return true;
        }
        var names = tags.Select(t => (t ?? string.Empty).Trim().TrimStart('#').ToLowerInvariant()).ToList();
        return names.Distinct().Count() == names.Count;
    }

    /// <summary>
    /// At most Validator.Hashtag.MaxQuantity tags
    /// </summary>
    public static bool MaxQuantity(List<string>? tags)
    {
        return tags == null || tags.Count <= Validator.Hashtag.MaxQuantity;
    }
}
