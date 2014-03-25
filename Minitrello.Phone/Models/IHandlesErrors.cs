namespace Minitrello.Phone.Models
{
    public interface IHandlesErrors
    {
         int ErrorCode { set; get; }
        string ErrorMessage { set; get; }
    }

}
