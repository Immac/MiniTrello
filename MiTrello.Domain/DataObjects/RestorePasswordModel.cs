namespace MiniTrello.Domain.DataObjects
{
    public class RestorePasswordModel:IHandlesErrors
    {
        public string Message { get; set; }
        public int ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
    }
}