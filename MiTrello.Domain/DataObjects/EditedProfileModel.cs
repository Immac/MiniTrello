namespace MiniTrello.Domain.DataObjects
{
    public class EditedProfileModel:IHandlesErrors
    {

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Message { get; set; }
        public int ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
    }
}