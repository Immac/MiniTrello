namespace PhoneApp1.DataObjects
{
    public class MemberModel:IHandlesErrors
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public int ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
    }
}