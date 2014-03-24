namespace PhoneApp1.DataObjects
{
    public class AccountAuthenticationModel
    {
        public int ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public string Token { get; set; }
        public string AccountName { get; set; }
    }
}