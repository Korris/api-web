using System.ComponentModel.DataAnnotations;

namespace Mcsg.Admin.Api.Requests
{
    public class CreateAdminReq
    {
        [EmailAddress]
        public string Email { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
