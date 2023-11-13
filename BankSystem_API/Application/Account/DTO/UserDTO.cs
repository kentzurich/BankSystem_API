namespace Application.Account.DTO
{
    public class UserDTO
    {
        public Guid AccountId { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public DateTime Birthdate { get; set; }
        public string Token { get; set; }
    }
}
