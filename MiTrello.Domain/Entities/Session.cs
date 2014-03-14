using System;
using MiniTrello.Domain.DataObjects;

namespace MiniTrello.Domain.Entities
{
    public class Session : IEntity,IHandlesErrors
    {
        public virtual bool IsRestoreSession { set; get; }
        public virtual string Token { get; set; }
        public virtual long Id { get; set; }
        public virtual DateTime DateStarted { set; get; }
        public virtual long Duration { set; get; }
        public virtual bool IsArchived { get; set; }
        public virtual Account SessionAccount { get; set; }
        public virtual int ErrorCode { get; set; }
        public virtual string ErrorMessage { get; set; }
    }
}