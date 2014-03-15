namespace MiniTrello.Domain.DataObjects
{
    public class BoardLogModel:IHandlesErrors
    {
        public string Log { set; get; }
        public int ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
    }
}