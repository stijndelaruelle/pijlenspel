using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ArrowCardGame
{
    public class VisualBoard : MonoBehaviour
    {
        [SerializeField]
        private int m_Width;

        [SerializeField]
        private int m_Height;

        [SerializeField]
        private List<VisualCardSlot> m_VisualCardSlots;

        private Board m_Board;
        public Board Board
        {
            get { return m_Board; }
        }

        private void Start()
        {
            List<CardSlot> cardSlots = new List<CardSlot>();
            foreach (VisualCardSlot visualCardSlot in m_VisualCardSlots)
            {
                cardSlots.Add(visualCardSlot.CardSlot);
            }

            m_Board = new Board(m_Width, m_Height, cardSlots);

            foreach (VisualCardSlot visualCardSlot in m_VisualCardSlots)
            {
                visualCardSlot.VisualCardSlotUpdatedEvent += OnCardSlotUpdated;
            }
        }

        private void OnDestroy()
        {
            if (m_VisualCardSlots == null)
                return;

            foreach (VisualCardSlot visualCardSlot in m_VisualCardSlots)
            {
                visualCardSlot.VisualCardSlotUpdatedEvent -= OnCardSlotUpdated;
            }
        }

        public ArrowResult Resolve(VisualDeck discardPile)
        {
            return m_Board.Resolve(discardPile.Deck);
        }

        public void Lock(bool state)
        {
            foreach (VisualCardSlot visualCardSlot in m_VisualCardSlots)
            {
                visualCardSlot.Lock(state);
            }
        }

        public void LockUsedSlots()
        {
            foreach (VisualCardSlot visualCardSlot in m_VisualCardSlots)
            {
                visualCardSlot.Lock(visualCardSlot.VisualCard != null);
            }
        }

        public VisualCardSlot GetVisualCardSlot(CardSlot cardSlot)
        {
            foreach (VisualCardSlot visualCardSlot in m_VisualCardSlots)
            {
                if (visualCardSlot.CardSlot == cardSlot)
                    return visualCardSlot;
            }

            return null;
        }

        private void OnCardSlotUpdated(VisualCardSlot visualCardSlot)
        {
            Table.Instance.PlayedCard(visualCardSlot.VisualCard != null);
            m_Board.Analyse();
        }
    }
}
