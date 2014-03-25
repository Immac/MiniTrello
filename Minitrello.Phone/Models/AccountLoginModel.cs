namespace Minitrello.Phone.Models
{
    public class AccountLoginModel
    {
        public int ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public long SessionDuration { get; set; }
    }
}