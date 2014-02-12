namespace MiniTrello.Domain.Entities
{
    public class Lane : IEntity
    {
        public virtual long Id { get; set; }
        public virtual bool IsArchived { get; set; }

    }
}