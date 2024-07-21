using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Lib.Data.Domain.Entities
{
    using Common;
    using Mcsg.Common.Core.Enums;

    [Table("SmartLookupUsers")]
    public class SmartLookupUser : BaseEntity
    {
        public Guid UserId { get; set; }
        public string? Keyword { get; set; }
        public LookupKeywordType KeywordType { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
