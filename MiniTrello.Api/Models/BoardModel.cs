using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using MiniTrello.Domain.Entities;

namespace MiniTrello.Api.Models
{
    public class BoardModel
    {
        private readonly IList<Lane> _lanes = new List<Lane>();
        private readonly IList<Account> _memberAccounts = new List<Account>();
        private readonly IList<Account> _administratorAccounts = new List<Account>();

        public long Id { get; set; }
        public string Title { get; set; }
        public bool IsArchived { get; set; }
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
                
    }
}