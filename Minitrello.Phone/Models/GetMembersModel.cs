using System.Collections.Generic;

namespace Minitrello.Phone.Models
{
    public class GetMembersModel:IHandlesErrors
    {
        private readonly List<MemberModel>  _members = new List<MemberModel>();

        public IEnumerable<MemberModel> Members
        {
            get { return _members; }
        }

        public void AddMember(MemberModel model)
        {
            if (!_members.Contains(model))
                _members.Add(model);
        }
        public int ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
    }
}