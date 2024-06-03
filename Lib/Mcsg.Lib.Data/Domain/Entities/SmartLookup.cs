using Mcsg.Lib.Data.Domain.Entities.Common;
using Mcsg.Lib.Data.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Lib.Data.Domain.Entities
{
    [Table("SmartLookups")]
    public class SmartLookup : BaseEntity
    {
        public string Keyword { get; set; }
        public LookupKeywordType KeywordType { get; set; } = LookupKeywordType.None;
        public int CountCriteria { get; set; }
    }
}
