namespace PhoneApp1.DataObjects
{
    public class RestorePasswordModel:IHandlesErrors
    {
        public string Message { get; set; }
        public int ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
    }
}