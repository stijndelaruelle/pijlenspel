using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ArrowCardGame
{
    public class VisualDeck : MonoBehaviour
    {
        [SerializeField]
        private DeckDefinition m_DeckDefinition;

        [SerializeField]
        private VisualCard m_VisualCardPrefab;
        private List<VisualCard> m_VisualCards;

        [SerializeField]
        private List<VisualCardSlot> m_VisualCardSlots;

        //Variables for the instantiated cards
        [SerializeField]
        private RectTransform m_TableRoot;

        [SerializeField]
        private VisualDeck m_PlayerHand;

        [SerializeField]
        private CanvasGroup m_CanvasGroup;

        private Deck m_Deck;
        public Deck Deck
        {
            get { return m_Deck; }
        }

        //Event
        private event VoidDelegate m_DrawCardEvent;
        public VoidDelegate DrawCardEvent
        {
            get { return m_DrawCardEvent; }
            set { m_DrawCardEvent = value; }
        }

        private void Start()
        {
            List<CardSlot> cardSlots = new List<CardSlot>();

            foreach (VisualCardSlot visualCardSlot in m_VisualCardSlots)
            {
                visualCardSlot.VisualCardSlotUpdatedEvent += OnCardSlotUpdated;
                cardSlots.Add(visualCardSlot.CardSlot);
            }

            if (m_DeckDefinition == null)
            {
                m_Deck = new Deck(cardSlots);
                return;
            }

            m_Deck = new Deck(m_DeckDefinition, cardSlots);

            m_VisualCards = new List<VisualCard>();

            List<Card> cards = m_Deck.Cards;
            for (int i = cards.Count - 1; i >= 0; --i)
            {
                VisualCard newVisualCard = GameObject.Instantiate(m_VisualCardPrefab) as VisualCard;

                newVisualCard.Initialize(cards[i], m_TableRoot, m_PlayerHand);

                VisualCardSlot emptyCardSlot = FirstEmptySlot();
                newVisualCard.SetVisualCardSlot(emptyCardSlot);
                newVisualCard.SetParent(emptyCardSlot.transform, 1.0f);

                m_VisualCards.Add(newVisualCard);
            }
        }

        private void OnDestroy()
        {
            foreach (VisualCardSlot visualCardSlot in m_VisualCardSlots)
            {
                visualCardSlot.VisualCardSlotUpdatedEvent -= OnCardSlotUpdated;
            }
        }

        public void AllowClicks(bool state)
        {
            m_CanvasGroup.blocksRaycasts = state;
        }

        public int NumberOfCards()
        {
            return m_Deck.Cards.Count;
        }

        private void OnCardSlotUpdated(VisualCardSlot updatedVisualCardSlot)
        {
            //Lame: But easy fix for now
            List<Card> cards = new List<Card>();

            foreach (VisualCardSlot visualCardSlot in m_VisualCardSlots)
            {
                //Exclude the decks SUPER LAME
                if (visualCardSlot.AllowMultipleCards)
                {
                    if (m_DrawCardEvent != null)
                        m_DrawCardEvent();
                    return;
                }
                    

                VisualCard visualCard = visualCardSlot.VisualCard;
                if (visualCard != null)
                    cards.Add(visualCard.Card);
            }

            m_Deck.Cards = cards;
        }

        public VisualCardSlot FirstEmptySlot()
        {
            foreach (VisualCardSlot visualCardSlot in m_VisualCardSlots)
            {
                if (visualCardSlot.IsEmpty() || visualCardSlot.AllowMultipleCards)
                    return visualCardSlot;
            }

            return null;
        }

        public VisualCardSlot GetVisualCardSlot(CardSlot cardSlot)
        {
            foreach(VisualCardSlot visualCardSlot in m_VisualCardSlots)
            {
                if (visualCardSlot.CardSlot == cardSlot)
                    return visualCardSlot;
            }

            return null;
        }
    }
}
