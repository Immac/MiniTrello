using System.Collections.Generic;
using MiniTrello.Domain.DataObjects;

namespace MiniTrello.Domain.Entities
{
    public class Board : IEntity
    {
        private readonly IList<Lane> _lanes = new List<Lane>();
        private readonly IList<AccountShell> _memberAccounts = new List<AccountShell>();
        private readonly IList<AccountShell> _administratorAccounts = new List<AccountShell>();

        public virtual Account OwnerAccount { set; get; }
        public virtual string Title { get; set; }
        public virtual long Id { get; set; }
        public virtual bool IsArchived { get; set; }

        public virtual IEnumerable<Lane> Lanes
        {
            get { return _lanes; }
        }
        public virtual IEnumerable<AccountShell> AdministratorAccounts
        {
            get { return _administratorAccounts; }
        }
        public virtual IEnumerable<AccountShell> MemberAccounts
        {
            get { return _memberAccounts; }
        }

        public virtual void AddLane(Lane lane)
        {
            if (!_lanes.Contains(lane))
            {
                _lanes.Add(lane);
            }
        }
        public virtual void AddMemberAccount(AccountShell member)
        {
            if (!_memberAccounts.Contains(member))
            {
                _memberAccounts.Add(member);    
            }
        }

        public virtual void AddAdministratorAccount(AccountShell admin)
        {
            if (!_administratorAccounts.Contains(admin))
            {
                _administratorAccounts.Add(admin);
            }
        }

        public virtual string Log { get; set; }
    }
}