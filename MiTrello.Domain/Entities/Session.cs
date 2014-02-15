using System;

namespace MiniTrello.Domain.Entities
{
    public class Session : IEntity
    {
        public virtual string Token { get; set; }
        public virtual long Id { get; set; }
        public virtual DateTime DateStarted { set; get; }
        public virtual long Duration { set; get; }
        public virtual bool IsArchived { get; set; }
        public virtual Account UserAccount { get; set; }
    }
}