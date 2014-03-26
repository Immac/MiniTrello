using System.Collections.Generic;

namespace MiniTrello.Domain.Entities
{
    public class AccountShell : IEntity
    {
        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }
        public virtual string Email { get; set; }

        public virtual long Id { get; set; }
        public virtual bool IsArchived { get; set; }
    }
}