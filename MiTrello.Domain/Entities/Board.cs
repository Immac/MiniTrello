using System.Collections.Generic;

namespace MiniTrello.Domain.Entities
{
    public class Board : IEntity
    {
        private readonly IList<Lane> _lanes = new List<Lane>();
        private readonly IList<Account> _memberAccounts = new List<Account>();
        private readonly IList<Account> _adminAccounts = new List<Account>();

        public virtual string Title { get; set; }
        public virtual long Id { get; set; }
        public virtual bool IsArchived { get; set; }

        public virtual IEnumerable<Lane> Lanes
        {
            get { return _lanes; }
        }public virtual IEnumerable<Account> AdminAccounts
        {
            get { return _adminAccounts; }
        }
        public virtual IEnumerable<Account> MemberAccounts
        {
            get { return _memberAccounts; }
        }

        public virtual void AddMember(Account member)
        {
            if (!_memberAccounts.Contains(member))
            {
                _memberAccounts.Add(member);    
            }
        }
    }
}