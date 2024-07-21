namespace Mcsg.Lib.Data.Domain.Entities.Interface
{
    public interface IAuditableEntity
    {
        public DateTime CreatedDate { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public Guid? LastModifiedBy { get; set; }
        public bool IsDelete { get; set; }
    }
}