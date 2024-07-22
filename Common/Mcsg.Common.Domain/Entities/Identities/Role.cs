using Microsoft.AspNetCore.Identity;

namespace Mcsg.Common.Domain.Entities
{
    public class Role : IdentityRole<Guid>
    {
        public string? DisplayName { get; set; }
    }
}