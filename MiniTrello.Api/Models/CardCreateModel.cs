namespace MiniTrello.Api.Models
{
    public class CardCreateModel
    {
        public long LaneId { set; get; }
        public string Name { set; get; }
        public string Content { set; get; }
    }
}