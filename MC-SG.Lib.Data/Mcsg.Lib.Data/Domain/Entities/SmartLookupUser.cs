using Mcsg.Lib.Data.Domain.Entities.Common;
using Mcsg.Lib.Data.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Lib.Data.Domain.Entities
{
    [Table("SmartLookupUsers")]
    public class SmartLookupUser : BaseEntity
    {
        public Guid UserId { get; set; }
        public string Keyword { get; set; }
        public LookupKeywordType KeywordType { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
