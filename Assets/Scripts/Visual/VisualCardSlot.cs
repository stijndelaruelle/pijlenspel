﻿using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

namespace ArrowCardGame
{
    public delegate void VoidDelegate();
    public delegate void IntDelegate();
    public delegate void VoidVisualCardSlotDelegate(VisualCardSlot visualCardSlot);

    public class VisualCardSlot : MonoBehaviour, IPointerDownHandler, IDropHandler
    {
        [SerializeField]
        private CanvasGroup m_CanvasGroup;

        [SerializeField]
        private bool m_FaceUp = true;
        public bool FaceUp
        {
            get { return m_FaceUp; }
        }

        private CardSlot m_CardSlot;
        public CardSlot CardSlot
        {
            get { return m_CardSlot; }
        }

        private VisualCard m_VisualCard;
        public VisualCard VisualCard
        {
            get { return m_VisualCard; }
            set
            {
                //Happens when the player places cards
                if (m_VisualCard != value)
                {
                    m_VisualCard = value;

                    //Update our data
                    if (m_VisualCard != null)
                    {
                        if (m_CardSlot.Card != value.Card)
                        {
                            m_CardSlot.Card = value.Card;
                            FireUpdateEvent();
                        }
                    }
                    else
                    {
                        FireUpdateEvent();
                    }
                }
            }
        }

        //Event
        private event VoidVisualCardSlotDelegate m_VisualCardSlotUpdatedEvent;
        public VoidVisualCardSlotDelegate VisualCardSlotUpdatedEvent
        {
            get { return m_VisualCardSlotUpdatedEvent; }
            set { m_VisualCardSlotUpdatedEvent = value; }
        }

        private void Awake()
        {
            m_CardSlot = new CardSlot();
        }

        public bool IsEmpty()
        {
            return (m_CardSlot.IsEmpty());
        }

        public void FireUpdateEvent()
        {
            if (m_VisualCardSlotUpdatedEvent != null)
                m_VisualCardSlotUpdatedEvent(this);
        }

        public void Lock(bool state)
        {
            m_CanvasGroup.blocksRaycasts = !state;
        }

        //----------------
        // INTERFACES
        //----------------

        //IPointerDownHandler
        public void OnPointerDown(PointerEventData eventData)
        {
            Debug.Log("We tapped the card");
        }

        //IDropHandler
        public void OnDrop(PointerEventData eventData)
        {
            //We only accept 1 card
            if (transform.childCount > 0)
                return;

            if (VisualCard.DraggedCard != null)
            {
                VisualCard.DraggedCard.SetVisualCardSlot(this);
            }
        }
    }
}
