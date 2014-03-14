using System.Collections.Generic;

namespace MiniTrello.Domain.DataObjects
{
    public class AccountModel
    {
        private readonly List<BoardTitleModel> _boards = new List<BoardTitleModel>();
        private readonly List<OrganizationNameModel> _organizations = new List<OrganizationNameModel>();
        
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public List<BoardTitleModel> Boards
        {
            get { return _boards; }
        }

        public List<OrganizationNameModel> Organizations
        {
            get { return _organizations; }
        }

        public void AddBoard(BoardTitleModel board)
        {
            _boards.Add(board);
        }
        public void AddOrganization(OrganizationNameModel organization)
        {
            _organizations.Add(organization);
        }

    }
}