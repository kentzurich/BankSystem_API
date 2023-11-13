namespace Application.Account.DTO
{
    public class AccountDTO
    {
        public Guid AccountId { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public double Balance { get; set; }
    }
}
