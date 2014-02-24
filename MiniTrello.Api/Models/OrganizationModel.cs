using System.Collections.Generic;

namespace MiniTrello.Api.Models
{
    public class OrganizationModel
    {
        private readonly List<BoardModel> _boards = new List<BoardModel>();

        public virtual List<BoardModel> Boards
        {
            get { return _boards; }
        }

        public virtual long Id { get; set; }
        public virtual bool IsArchived { get; set; }
        public virtual string Name { set; get; }
        public virtual string Description { set; get; }

        public virtual void AddBoard(BoardModel board)
        {
            _boards.Add(board);
        }

    }
}