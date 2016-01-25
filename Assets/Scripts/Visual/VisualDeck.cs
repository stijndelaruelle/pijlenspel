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
        private VisualCardSlot m_VisualCardSlotPrefab;

        [SerializeField]
        private int m_MinNumberOfVisualCardSlots = 0;

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

        private List<VisualCardSlot> m_VisualCardSlots;

        //Event
        private event VoidDelegate m_DrawCardEvent;
        public VoidDelegate DrawCardEvent
        {
            get { return m_DrawCardEvent; }
            set { m_DrawCardEvent = value; }
        }

        private void Awake()
        {
            m_VisualCardSlots = new List<VisualCardSlot>();
            m_VisualCards = new List<VisualCard>();
        }

        private void Start()
        {
            m_Deck = new Deck(m_DeckDefinition);
            m_Deck.CardSlotAddedEvent += OnCardSlotDataAdded;

            for (int i = 0; i < m_MinNumberOfVisualCardSlots; ++i)
            {
                VisualCardSlot visualCardSlot = AddVisualCardSlot();
                visualCardSlot.VisualCardSlotUpdatedEvent += OnCardSlotUpdated;
            }

            //Create all the cards
            if (m_DeckDefinition == null)
                return;

            List<Card> cards = m_Deck.Cards;
            for (int i = cards.Count - 1; i >= 0; --i)
            {
                VisualCard newVisualCard = GameObject.Instantiate(m_VisualCardPrefab) as VisualCard;

                newVisualCard.Initialize(cards[i], m_TableRoot, m_PlayerHand);

                VisualCardSlot emptyCardSlot;
                
                if (i < m_VisualCardSlots.Count)
                {
                    emptyCardSlot = FirstEmptySlot();
                }
                else
                {
                    emptyCardSlot = AddVisualCardSlot();
                }

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

            if (m_Deck != null)
                m_Deck.CardSlotAddedEvent -= OnCardSlotDataAdded;
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
                if (m_DrawCardEvent != null)
                    m_DrawCardEvent();

                VisualCard visualCard = visualCardSlot.VisualCard;
                if (visualCard != null)
                    cards.Add(visualCard.Card);
            }

            m_Deck.Cards = cards;
        }

        private void OnCardSlotDataAdded()
        {
            AddVisualCardSlot();
        }

        public VisualCardSlot FirstEmptySlot()
        {
            foreach (VisualCardSlot visualCardSlot in m_VisualCardSlots)
            {
                if (visualCardSlot.IsEmpty())
                    return visualCardSlot;
            }

            //No empty slots were found create a new one
            return AddVisualCardSlot();
        }

        private VisualCardSlot AddVisualCardSlot()
        {
            VisualCardSlot visualCardSlot = GameObject.Instantiate(m_VisualCardSlotPrefab) as VisualCardSlot;
            visualCardSlot.VisualCardSlotUpdatedEvent += OnCardSlotUpdated;

            visualCardSlot.gameObject.transform.SetParent(transform);
            visualCardSlot.gameObject.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
            visualCardSlot.gameObject.transform.localRotation = Quaternion.identity;
            visualCardSlot.gameObject.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

            visualCardSlot.gameObject.name = gameObject.name + ": Cardslot";
            m_VisualCardSlots.Add(visualCardSlot);

            m_Deck.AddCardSlot(visualCardSlot.CardSlot);

            return visualCardSlot;
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
