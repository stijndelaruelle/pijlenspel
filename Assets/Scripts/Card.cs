using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ArrowCardGame
{
    public delegate void VoidBoolDirectionDelegate(bool state, Direction dir);

    public class Card
    {
        private CardDefinition m_CardDefinition;
        public CardDefinition CardDefinition
        {
            get { return m_CardDefinition; }
        }

        private CardSlot m_CardSlot;
        private bool m_IsRotated = false;

        //Events
        private event VoidBoolDirectionDelegate m_CardAnalysedEvent;
        public VoidBoolDirectionDelegate CardAnalysedEvent
        {
            get { return m_CardAnalysedEvent; }
            set { m_CardAnalysedEvent = value; }
        }

        public Card(CardDefinition cardDefinition)
        {
            m_CardDefinition = cardDefinition;
        }

        public void SetCardSlot(CardSlot cardSlot)
        {
            //Reset old cardslot
            if (m_CardSlot != null)
                m_CardSlot.SetCard(null);

            m_CardSlot = cardSlot;

            if (cardSlot != null)
                cardSlot.SetCard(this);
        }

        public bool HasArrow(Direction dir)
        {
            if (m_IsRotated)
            {
                dir = Table.RotateDir(dir);
            }

            foreach (CardArrow arrow in m_CardDefinition.Arrows)
            {
                if (arrow.Direction == dir)
                {
                    return true;
                }
            }

            return false;
        }

        public void CardAnalysed(bool state, Direction dir)
        {
            //Used to forward visual changes
            if (m_CardAnalysedEvent != null)
                m_CardAnalysedEvent(state, dir);
        }

        public List<CardArrow> GetArrows(Direction dir)
        {
            if (m_IsRotated)
            {
                dir = Table.RotateDir(dir);
            }

            List<CardArrow> arrows = new List<CardArrow>();
            foreach (CardArrow arrow in m_CardDefinition.Arrows)
            {
                if (arrow.Direction == dir)
                {
                    arrows.Add(arrow);
                }
            }

            return arrows;
        }
    }
}
