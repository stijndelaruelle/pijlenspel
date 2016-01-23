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

        private Deck m_Deck;
        public Deck Deck
        {
            get { return m_Deck; }
        }

        private void Start()
        {
            foreach (VisualCardSlot visualCardSlot in m_VisualCardSlots)
            {
                visualCardSlot.VisualCardSlotUpdatedEvent += OnCardSlotUpdated;
            }

            if (m_DeckDefinition == null)
            {
                m_Deck = new Deck();
                return;
            }

            m_Deck = new Deck(m_DeckDefinition);
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

        private void OnCardSlotUpdated(VisualCardSlot updatedVisualCardSlot)
        {
            //Lame but works for now
            List<Card> cards = new List<Card>();
            foreach (VisualCardSlot visualCardSlot in m_VisualCardSlots)
            {
                if (visualCardSlot.VisualCard != null)
                    cards.Add(visualCardSlot.VisualCard.Card);
            }

            m_Deck.SetCards(cards);
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
