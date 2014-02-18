using System.Collections.Generic;
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
            get { return oSerializer.Serialize(Lanes); }
        }
        public virtual string AdministratorAccountsString
        {
            get { return oSerializer.Serialize(AdministratorAccounts); }
        }
        public virtual string MemberAccountsString
        {
            get { return oSerializer.Serialize(MemberAccounts); }
        }

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