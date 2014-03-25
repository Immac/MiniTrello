using System.Collections.Generic;

namespace Minitrello.Phone.Models
{
    public class GetBoardsModel
    {
        private readonly List<BoardModel> _boards = new List<BoardModel>();
        public int ErrorCode { set; get; }
        public string ErrorMessage { set; get; }
        public IEnumerable<BoardModel> Boards
        {
            get { return _boards; }
        }

        public void AddBoard(BoardModel board)
        {
            _boards.Add(board);
        }
    }
}