namespace MiniTrello.Api.Models
{
    public class CardModel
    {
        public long Id { get; set; }
        public bool IsArchived { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }
    }
}