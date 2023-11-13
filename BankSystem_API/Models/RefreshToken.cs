namespace Models
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public UserAccount UserAccount { get; set; }
        public string Token { get; set; }
        public DateTime Expires { get; set; } = DateTime.UtcNow.AddDays(7);
        public bool IsExpired => DateTime.UtcNow >= Expires;
        public DateTime? Revoke { get; set; }
        public bool IsActive => Revoke == null && !IsExpired;
    }
}
