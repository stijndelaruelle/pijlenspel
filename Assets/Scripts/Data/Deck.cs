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

        //Event
        private event VoidDelegate m_CardSlotAddedEvent;
        public VoidDelegate CardSlotAddedEvent
        {
            get { return m_CardSlotAddedEvent; }
            set { m_CardSlotAddedEvent = value; }
        }

        //Used for resetting the game
        private List<Card> m_BackupCards;

        private List<CardSlot> m_CardSlots;

        public Deck(DeckDefinition deckDefinition)
        {   
            m_Cards = new List<Card>();
            m_BackupCards = new List<Card>();

            //Create all the cards
            if (deckDefinition != null)
            {
                foreach (CardDefinition cardDef in deckDefinition.CardDefinitions)
                {
                    Card newCard = new Card(cardDef);
                    m_Cards.Add(newCard);
                    m_BackupCards.Add(newCard);
                }
            }
        }

        public void AddCard(Card card)
        {
            if (m_Cards.Contains(card))
                return;

            m_Cards.Add(card);
            card.CardSlot = FirstEmptySlot();
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
                Card card = m_Cards[m_Cards.Count - 1];
                m_Cards.Remove(card);
                return card;
            }

            Debug.LogWarning("Drawing from a deck with no cards!");
            return null;
        }

        public void ResetDeck()
        {
            m_Cards.Clear();

            for (int i = m_BackupCards.Count - 1; i >= 0; --i)
            {
                Card card = m_BackupCards[i];
                card.CardSlot = null;
                AddCard(card);
            }
        }

        public void ShuffleDeck()
        {
            //Shuffle the entire deck
            m_Cards.Shuffle();

            //Sort the tiers
            //...
        }


        public void AddCardSlot(CardSlot cardSlot)
        {
            if (m_CardSlots == null)
                m_CardSlots = new List<CardSlot>();

            m_CardSlots.Add(cardSlot);
        }

        public CardSlot FirstEmptySlot()
        {
            foreach (CardSlot cardSlot in m_CardSlots)
            {
                if (cardSlot.IsEmpty())
                    return cardSlot;
            }

            return null;
        }
    }
}
