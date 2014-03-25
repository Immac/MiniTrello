namespace Minitrello.Phone.Models
{
    public class PasswordRestoreInputModel:IHandlesErrors
    {
        public string Email { set; get; }
        public int ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
    }
}