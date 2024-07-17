using Microsoft.AspNetCore.Identity;

namespace Mcsg.Lib.Data.Domain.Entities
{
    public class Role : IdentityRole<Guid>
    {
        public string? DisplayName { get; set; }
    }
}