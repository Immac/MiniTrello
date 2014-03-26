using System.Collections.Generic;

namespace MiniTrello.Domain.Entities
{
    public class Account : IEntity
    {
        private readonly IList<Board>         _boards        =  new List<Board>();
        private readonly IList<Organization>  _organizations =  new List<Organization>();
        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }
        public virtual string Email { get; set; }
        public virtual string Password { get; set; }
        
        public virtual IEnumerable<Board> Boards
        {
            get { return _boards; }
        }

        public virtual IEnumerable<Organization> Organizations
        {
            get { return _organizations; }
        }

        public virtual long Id { get; set; }
        public virtual bool IsArchived { get; set; }

        public virtual void AddBoard(Board board)
        {
            if (_boards.Contains(board)) return;
            board.OwnerAccount = this;
            board.AddAdministratorAccount(new AccountShell
            {
                Email = Email,
                LastName = LastName,
                FirstName = FirstName,

            });
            board.AddMemberAccount(new AccountShell
            {
                Email = Email,
                LastName = LastName,
                FirstName = FirstName,

            });
            _boards.Add(board);
        }

        public virtual void AddOrganization(Organization organization)
        {
            if (_organizations.Contains(organization)) return;
            _organizations.Add(organization);
        }
    }
}