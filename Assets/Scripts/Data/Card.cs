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
        public CardSlot CardSlot
        {
            get { return m_CardSlot; }
            set
            {
                if (m_CardSlot != value)
                {
                    //Remove me from the old slot
                    if (m_CardSlot != null)
                        m_CardSlot.Card = null;

                    m_CardSlot = value;
                    
                    if (m_CardSlot != null)
                        m_CardSlot.Card = this;

                    if (m_CardSlotUpdatedEvent != null)
                        m_CardSlotUpdatedEvent();
                }
            }
        }

        private bool m_IsRotated = false;
        public bool IsRotated
        {
            get { return m_IsRotated; }
        }

        //Event
        private event VoidDelegate m_CardSlotUpdatedEvent;
        public VoidDelegate CardSlotUpdatedEvent
        {
            get { return m_CardSlotUpdatedEvent; }
            set { m_CardSlotUpdatedEvent = value; }
        }

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

        public void Rotate()
        {
            m_IsRotated = !m_IsRotated;
        }

        public void Rotate(bool value)
        {
            m_IsRotated = value;
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

        public List<CardArrow> GetArrows()
        {
            return m_CardDefinition.Arrows;
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

        public void CardAnalysed(bool state, Direction dir)
        {
            //Used to forward visual changes (only way we communicate upwards!)
            //Want to change this as we don't want to communcate up visually.
            if (m_CardAnalysedEvent != null)
                m_CardAnalysedEvent(state, dir);
        }

    }
}
