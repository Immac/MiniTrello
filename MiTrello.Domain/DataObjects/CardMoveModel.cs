namespace MiniTrello.Domain.DataObjects
{
    public class CardMoveModel:IHandlesErrors
    {
        public long DestinationId { set; get; }
        public long CardId { set; get; }
        public int ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
    }
}