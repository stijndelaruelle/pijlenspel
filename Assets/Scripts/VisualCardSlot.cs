using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

namespace ArrowCardGame
{
    public delegate void VoidDelegate();

    public class VisualCardSlot : MonoBehaviour, IPointerDownHandler, IDropHandler
    {
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

        //Event
        private event VoidDelegate m_CardSlotUpdatedEvent;
        public VoidDelegate CardSlotUpdatedEvent
        {
            get { return m_CardSlotUpdatedEvent; }
            set { m_CardSlotUpdatedEvent = value; }
        }

        private void Awake()
        {
            m_CardSlot = new CardSlot();
        }

        public bool IsEmpty()
        {
            return (transform.childCount == 0);
        }

        public void UpdateCardSlot()
        {
            //We're taking away the card.
            if (m_CardSlotUpdatedEvent != null)
                m_CardSlotUpdatedEvent();
        }

        //----------------
        // INTERFACES
        //----------------

        //IPointerDownHandler
        public void OnPointerDown(PointerEventData eventData)
        {
            Debug.Log("We tapped the board");
        }

        //IDropHandler
        public void OnDrop(PointerEventData eventData)
        {
            //We drop a card on here
            //Debug.Log("We dropped a card on the board!");

            //We only accept 1 card
            if (transform.childCount > 0)
                return;

            if (VisualCard.m_DraggedCard != null)
            {
                VisualCard.m_DraggedCard.SetVisualCardSlot(this);
            }
        }
    }
}
