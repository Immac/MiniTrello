using System.Collections.Generic;

namespace MiniTrello.Domain.DataObjects
{
    public class GetOrganizationsModel : IHandlesErrors
    {
        public int ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        private readonly List<OrganizationNameDescriptionModel> _nameDescriptionList = new List<OrganizationNameDescriptionModel>();
        public IEnumerable<OrganizationNameDescriptionModel> NamesDescription
        {
            get { return _nameDescriptionList; }
        }

        public void AddNameDescription(OrganizationNameDescriptionModel nameDescriptionModel)
        {
            _nameDescriptionList.Add(nameDescriptionModel);
        }
    }
}