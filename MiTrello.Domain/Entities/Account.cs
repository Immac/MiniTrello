using System.Collections.Generic;

namespace MiniTrello.Domain.Entities
{
    public class Account : IEntity
    {
        private readonly IList<Board> _boards = new List<Board>();
        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }
        public virtual string Email { get; set; }
        public virtual string Password { get; set; }

        public virtual IEnumerable<Board> Boards
        {
            get { return _boards; }
        }

        public virtual long Id { get; set; }
        public virtual bool IsArchived { get; set; }

        public virtual void AddBoard(Board board)
        {
            if (_boards.Contains(board))
            {
                _boards.Add(board);
            }
        }
    }
}