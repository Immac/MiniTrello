namespace PhoneApp1.DataObjects
{
    public interface IHandlesErrors
    {
         int ErrorCode { set; get; }
        string ErrorMessage { set; get; }
    }

}
