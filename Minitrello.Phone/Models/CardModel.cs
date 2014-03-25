namespace Minitrello.Phone.Models
{
    public class CardModel:IHandlesErrors
    {
        public long Id { get; set; }
        public bool IsArchived { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }
        public int ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
    }
}