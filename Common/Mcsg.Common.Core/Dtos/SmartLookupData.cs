namespace Mcsg.Common.Core.Dtos;

using Enums;

public class SmartLookupData
{
    public string ProfileName { get; set; }
    public List<string> Tags { get; set; }
    public LookupKeywordType KeywordType { get; set; }
}
