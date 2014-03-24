namespace MiniTrello.Domain.DataObjects
{
    public class OrganizationNameDescriptionModel: IHandlesErrors
    {
        public string Name { set; get; }
        public string Description { set; get; }

        public int ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
    }
}
