using System;

namespace MiniTrello.Domain.Entities
{
    public class Session : IEntity
    {
        public virtual string Token { get; set; }
        public virtual long Id { get; set; }
        public virtual DateTime DateStarted { set; get; }
        public virtual TimeSpan Duration { set; get; }
        public virtual bool IsArchived { get; set; }
    }
}