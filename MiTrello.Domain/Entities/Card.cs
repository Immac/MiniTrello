using System;

namespace MiniTrello.Domain.Entities
{
    public class Card : IEntity
    {
        public virtual long Id { get; set; }
        public virtual bool IsArchived { get; set; }
        public virtual string Name { get; set; }
        public virtual string Content { get; set; }
    }

    public class Session : IEntity
    {
        public virtual string Token { get; set; }
        public virtual long Id { get; set; }
        public virtual DateTime DateStarted { set; get; }
        public virtual TimeSpan Duration { set; get; }
        public virtual bool IsArchived { get; set; }
    }
}