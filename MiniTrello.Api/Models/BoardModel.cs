using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web.Helpers;
using System.Web.Script.Serialization;
using MiniTrello.Domain.Entities;

namespace MiniTrello.Api.Models
{
    public class BoardModel
    {
        private readonly List<LaneModel> _lanes = new List<LaneModel>();
        private readonly List<AccountModel> _memberAccounts = new List<AccountModel>();
        private readonly List<AccountModel> _administratorAccounts = new List<AccountModel>();
        public string Log { set; get; }
        public long Id { get; set; }
        public string Title { get; set; }
        public bool IsArchived { get; set; }
        public string Name { get; set; }
        
        public List<LaneModel> Lanes
        {
            get { return _lanes; }
        }
        public List<AccountModel> AdministratorAccounts
        {
            get { return _administratorAccounts; }
        }
        public List<AccountModel> MemberAccounts
        {
            get { return _memberAccounts; }
        }

        public void AddLane(LaneModel lane)
        {
            _lanes.Add(lane);
        }

        public void AddMemberAccount(AccountModel memberAccount)
        {
            _memberAccounts.Add(memberAccount);
        }

        public void AddAdmininstratorAccount(AccountModel administratorAccount)
        {
            _administratorAccounts.Add(administratorAccount);
        }
       
    }
}