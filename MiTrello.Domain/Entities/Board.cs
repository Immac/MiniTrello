using System.Collections;
using System.Collections.Generic;

namespace MiniTrello.Domain.Entities
{
    public class Board : IEntity
    {
        private readonly IList<Lane> _laneList = new List<Lane>();
        private readonly IList<Account> _accounts = new List<Account>();
        public virtual string Title { get; set; }
        public virtual long Id { get; set; }
        public virtual bool IsArchived { get; set; }

        public virtual IEnumerable<Lane> Lanes
        {
            get { return _laneList; }
        }

        public virtual IEnumerable<Account> Accounts
        {
            get { return _accounts; }
        }

        public virtual void AddMember(Account member)
        {
            if (!_accounts.Contains(member))
            {
                _accounts.Add(member);    
            }
        }
    }
}