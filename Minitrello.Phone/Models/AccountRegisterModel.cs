namespace Minitrello.Phone.Models
{
    public class AccountRegisterModel:IHandlesErrors
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public int ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
    }
}