using System.Collections.Generic;

namespace MiniTrello.Api.Models
{
    public class LaneModel
    {
        public long Id { get; set; }
        public bool IsArchived { get; set; }

        private readonly List<CardModel> _cards = new List<CardModel>();

        public List<CardModel> Cards
        {
            get { return _cards; }
        }

        public void AddCard(CardModel cardModel)
        {
            _cards.Add(cardModel);
        }

    }
}