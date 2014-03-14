namespace MiniTrello.Domain.DataObjects
{
    public class AccountAuthenticationModel
    {
        public int ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public string Token { get; set; }
    }
}