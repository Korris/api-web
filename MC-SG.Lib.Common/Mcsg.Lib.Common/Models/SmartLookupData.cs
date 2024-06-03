using Mcsg.Lib.Data.Enums;

namespace Mcsg.Lib.Common.Models
{
    public class SmartLookupData
    {
        public string ProfileName { get; set; }
        public List<string> Tags { get; set; }
        public LookupKeywordType KeywordType { get; set; }
    }
}
