using System.Collections.Generic;

namespace MiniTrello.Domain.Entities
{
    public class Account : IEntity
    {
        private readonly IList<Board> _ownedBoards = new List<Board>();
        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }
        public virtual string Email { get; set; }
        public virtual string Password { get; set; }

        public virtual IEnumerable<Board> OwnedBoards
        {
            get { return _ownedBoards; }
        }


        public virtual long Id { get; set; }
        public virtual bool IsArchived { get; set; }

        public virtual void AddBoard(Board board)
        {
            if (_ownedBoards.Contains(board))
            {
                _ownedBoards.Add(board);
            }
        }
    }
}