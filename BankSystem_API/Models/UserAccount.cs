using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public class UserAccount : IdentityUser
    {
        public Guid AccountId { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        //[Column(TypeName = "Date")]
        public DateTime Birthdate { get; set; }
        public int Balance { get; set; } = 0;
        public bool IsActive { get; set; } = true;
        public DateTime DateTimeCreated { get; set; } = DateTime.UtcNow;
        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    }
}
