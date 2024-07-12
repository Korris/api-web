namespace Mcsg.Lib.Common.Models;

using Mcsg.Common.Core.Enums;

public class SmartLookupData
{
    public string ProfileName { get; set; }
    public List<string> Tags { get; set; }
    public LookupKeywordType KeywordType { get; set; }
}
