namespace MiniTrello.Domain.DataObjects
{
    public class AddMemberBoardModel
    {
        public string MemberEmail { get; set; }
        public long BoardID { get; set; }
    }
}