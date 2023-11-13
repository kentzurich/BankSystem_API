using System.ComponentModel.DataAnnotations;

namespace Application.Account.DTO
{
    public class RegisterAccountDTO
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string MiddleName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public DateTime Birthdate { get; set; }
        [Required]
        [EmailAddress]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
