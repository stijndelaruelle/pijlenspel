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

        //Used for resetting the game
        private List<Card> m_BackupCards;

        private List<CardSlot> m_CardSlots;

        public Deck(List<CardSlot> cardSlots)
        {
            m_Cards = new List<Card>();
            m_CardSlots = cardSlots;
        }

        public Deck(DeckDefinition deckDefinition, List<CardSlot> cardSlots)
        {
            m_Cards = new List<Card>();
            m_BackupCards = new List<Card>();

            //Create all the cards
            foreach (CardDefinition cardDef in deckDefinition.CardDefinitions)
            {
                Card newCard = new Card(cardDef);
                m_Cards.Add(newCard);
                m_BackupCards.Add(newCard);
            }

            m_CardSlots = cardSlots;
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
                Card card = m_Cards[0];
                m_Cards.Remove(card);
                return card;
            }

            Debug.LogError("Drawing from a deck with no cards!");
            return null;
        }

        public void ResetDeck()
        {
            m_Cards.Clear();

            foreach(Card card in m_BackupCards)
            {
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
