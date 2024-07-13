namespace Mcsg.Common.Core.Dtos;

using Enums;

/// <summary>
/// SmartLookup data transfer object
/// </summary>
public class SmartLookupDto
{
    /// <summary>
    /// Profile name
    /// </summary>
    public string ProfileName { get; set; } = default!;

    /// <summary>
    /// Tags
    /// </summary>
    public List<string> Tags { get; set; } = [];

    /// <summary>
    /// Keyword type
    /// </summary>
    public LookupKeywordType KeywordType { get; set; }
}
