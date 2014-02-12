namespace MiniTrello.Domain.Entities
{
    public class Card : IEntity
    {
        public long Id { get; set; }
        public bool IsArchived { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }
    }
}