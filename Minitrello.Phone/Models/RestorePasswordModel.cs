namespace Minitrello.Phone.Models
{
    public class RestorePasswordModel:IHandlesErrors
    {
        public string Message { get; set; }
        public int ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
    }
}