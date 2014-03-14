namespace MiniTrello.Domain.DataObjects
{
    public class BoardDeleteModel
    {
        public long Id { get; set; }
        public bool IsArchived { get; set; }
    }
}