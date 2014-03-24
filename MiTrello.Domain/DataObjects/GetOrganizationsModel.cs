using System.Collections.Generic;

namespace MiniTrello.Domain.DataObjects
{
    public class GetOrganizationsModel : IHandlesErrors
    {
        public int ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        private readonly List<string> _nameList = new List<string>();
        public IEnumerable<string> Names
        {
            get { return _nameList; }
        }
      
        public void AddName(string name)
        {
            _nameList.Add(name);
        }
    }
}