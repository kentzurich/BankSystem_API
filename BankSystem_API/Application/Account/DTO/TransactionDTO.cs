namespace Application.Account.DTO
{
    public class TransactionDTO
    {
        public Guid SenderAccountId { get; set; }
        public string SenderName { get; set; }
        public Guid ReceiverAccountId { get; set; }
        public string ReceiverName { get; set; }
        public int Amount { get; set; }
    }
}
