using System.Collections.Generic;

namespace MiniTrello.Domain.Entities
{
    public class Organization : IEntity
    {
        private readonly IList<Board>  _boards = new List<Board>();
        public virtual Account OwnerAccount { set; get; }
        public virtual long Id { get; set; }
        public virtual bool IsArchived { get; set; }
        public virtual string Name { set; get; }
        public virtual string Description { set; get; }
        public virtual void AddBoard(Board board)
        {
            if (!_boards.Contains(board))
            {
                _boards.Add(board);
            }
        }
    }
}