using System.ComponentModel.DataAnnotations;

namespace Application.Account.DTO
{
    public class LoginAccountDTO
    {
        [Required]
        [EmailAddress]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
