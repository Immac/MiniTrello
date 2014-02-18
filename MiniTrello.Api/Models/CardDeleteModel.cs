namespace MiniTrello.Api.Models
{
    public class CardDeleteModel
    {
        public long CardId { set; get; }
        public bool IsArchived { set; get; }
    }
}