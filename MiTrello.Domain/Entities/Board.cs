﻿using System.Collections.Generic;

namespace MiniTrello.Domain.Entities
{
    public class Board : IEntity
    {
        private readonly IList<Lane> _lanes = new List<Lane>();
        
        private readonly IList<Account> _memberAccounts = new List<Account>();
        private readonly IList<Account> _administratorAccounts = new List<Account>();

        public virtual Account OwnerAccount { set; get; }
        public virtual string Title { get; set; }
        public virtual long Id { get; set; }
        public virtual bool IsArchived { get; set; }

        public virtual IEnumerable<Lane> Lanes
        {
            get { return _lanes; }
        }
        public virtual IEnumerable<Account> AdministratorAccounts
        {
            get { return _administratorAccounts; }
        }
        public virtual IEnumerable<Account> MemberAccounts
        {
            get { return _memberAccounts; }
        }

        public virtual void AddMemberAccount(Account member)
        {
            if (!_memberAccounts.Contains(member))
            {
                _memberAccounts.Add(member);    
            }
        }

        public virtual void AddAdministratorAccount(Account admin)
        {
            if (!_administratorAccounts.Contains(admin))
            {
                _administratorAccounts.Add(admin);
            }
        }
    }
}