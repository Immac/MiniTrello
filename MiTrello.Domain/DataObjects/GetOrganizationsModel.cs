using System.Collections.Generic;

namespace MiniTrello.Domain.DataObjects
{
    public class GetOrganizationsModel : IHandlesErrors
    {
        public int ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        private readonly List<string> _nameList = new List<string>();
        private readonly List<BoardModel> _boardModels = new List<BoardModel>();
        public IEnumerable<string> Names
        {
            get { return _nameList; }
        }

        public IEnumerable<BoardModel> Boards
        {
            get { return _boardModels; }
        }

        public void AddBoard(BoardModel model)
        {
            if(!_boardModels.Contains(model))
                _boardModels.Add(model);
        }
        public void AddName(string name)
        {
            _nameList.Add(name);
        }
    }
}