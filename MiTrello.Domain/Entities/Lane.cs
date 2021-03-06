﻿using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace MiniTrello.Domain.Entities
{
    public class Lane : IEntity
    {
        public virtual long Id { get; set; }
        public virtual bool IsArchived { get; set; }

        public virtual string Name { get; set; }

        private readonly IList<Card> _cards = new List<Card>();

        public virtual IEnumerable<Card> Cards
        {
            get { return _cards; }
        }

        public virtual void AddCard(Card card)
        {
            if (!_cards.Contains(card))
            {
                _cards.Add(card);
            }
        }

        public virtual void RemoveCard(Card card)
        {
            _cards.Remove(card);
        }
    }
}