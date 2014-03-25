using System.Collections.Generic;

namespace Minitrello.Phone.Models
{
    public class LaneModel:IHandlesErrors
    {
        public long Id { get; set; }
        public bool IsArchived { get; set; }

        public string Name { set; get; }

        private readonly List<CardModel> _cards = new List<CardModel>();

        public List<CardModel> Cards
        {
            get { return _cards; }
        }

        public void AddCard(CardModel cardModel)
        {
            _cards.Add(cardModel);
        }

        public int ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
    }
}