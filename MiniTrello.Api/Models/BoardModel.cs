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
        JavaScriptSerializer oSerializer = new JavaScriptSerializer();
        private readonly IList<Lane> _lanes = new List<Lane>();
        private readonly IList<Account> _memberAccounts = new List<Account>();
        private readonly IList<Account> _administratorAccounts = new List<Account>();
        public string Log { set; get; }
        public long Id { get; set; }
        public string Title { get; set; }
        public bool IsArchived { get; set; }
        public virtual string LanesString
        {
            get
            {
                string returnString = _lanes.Aggregate("", (current, lane) => current + lane.Id.ToString(CultureInfo.InvariantCulture));
                return oSerializer.Serialize(returnString);
            }
        }
        public virtual string AdministratorAccountsString
        {
            get
            {
                string returnString = _administratorAccounts.Aggregate("", (current, adminAccount) => current +adminAccount.Email);
                return oSerializer.Serialize(returnString);
                
            }
        }
        public virtual string MemberAccountsString
        {
            get
            {
                string retrunString = _memberAccounts.Aggregate("", (current, memberAccount) => current + memberAccount.Email);
                return oSerializer.Serialize(retrunString);
            }
        }
        /*
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
        }*/

        public virtual void AddLane(Lane lane)
        {
            if(!_lanes.Contains(lane))
            _lanes.Add(lane);
        }
        public virtual void AddMemberAccount(Account member)
        {
            if(!_memberAccounts.Contains(member))
            _memberAccounts.Add(member);
        }

        public virtual void AddAdministratorAccount(Account administraror)
        {
            if (!_administratorAccounts.Contains(administraror))
            {
                _memberAccounts.Add(administraror);
            }
        }
                
    }
}