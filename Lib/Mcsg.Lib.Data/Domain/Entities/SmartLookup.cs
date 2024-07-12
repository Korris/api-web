using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Lib.Data.Domain.Entities
{
    using Common;
    using Mcsg.Common.Core.Enums;

    [Table("SmartLookups")]
    public class SmartLookup : BaseEntity
    {
        public string? Keyword { get; set; }
        public LookupKeywordType KeywordType { get; set; } = LookupKeywordType.None;
        public int CountCriteria { get; set; }
    }
}
