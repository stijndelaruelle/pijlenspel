using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ArrowCardGame
{
    public delegate void VoidCardDelegate(Card card);

    //A collection of cards: Decks, Hands, Discard piles... anything!
    public class Deck
    {
        private List<Card> m_Cards;
        public List<Card> Cards
        {
            get { return m_Cards; }
            set { m_Cards = value; }
        }

        private VoidCardDelegate m_CardAddedEvent;
        public VoidCardDelegate CardAddedEvent
        {
            get { return m_CardAddedEvent; }
            set { m_CardAddedEvent = value; }
        }

        public Deck()
        {
            m_Cards = new List<Card>();
        }

        public Deck(DeckDefinition deckDefinition)
        {
            m_Cards = new List<Card>();

            //Create all the cards
            foreach (CardDefinition cardDef in deckDefinition.CardDefinitions)
            {
                Card newCard = new Card(cardDef);
                m_Cards.Add(newCard);
            }
        }

        public void AddCard(Card card)
        {
            if (m_Cards.Contains(card))
                return;

            m_Cards.Add(card);

            if (m_CardAddedEvent != null)
                m_CardAddedEvent(card);
        }

        public void RemoveCard(Card card)
        {
            if (m_Cards.Contains(card) == false)
                return;

            m_Cards.Remove(card);
        }

        public Card DrawCard()
        {
            if (m_Cards.Count > 0)
            {
                Card card = m_Cards[0];
                m_Cards.Remove(card);
                return card;
            }

            Debug.LogError("Drawing from a deck with no cards!");
            return null;
        }

        public void ShuffleDeck()
        {
            //Shuffle the entire deck
            m_Cards.Shuffle();

            //Sort the tiers
            //...
        }
    }
}
