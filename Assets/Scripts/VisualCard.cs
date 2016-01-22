using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

namespace ArrowCardGame
{
    public class VisualCard : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public static VisualCard m_DraggedCard = null;

        [SerializeField]
        private Image m_Image;

        [SerializeField]
        private List<VisualArrow> m_Arrows;

        private Card m_Card;
        public Card Card
        {
            get { return m_Card; }
        }

        private VisualCardSlot m_VisualCardSlot;
        private VisualCardSlot m_LastVisualCardSlot;

        private RectTransform m_TableRoot;
        private CardSlotGrid m_DefaultCardSlotGrid;
        
        private RectTransform m_CanvasTransform; //The canvas transform, used for drag & drop
        private CanvasGroup m_CanvasGroup;

        private bool m_IsInitialized = false;
        private bool m_FaceUp = false;


        private void Awake()
        {
            //Visual stuff
            m_CanvasTransform = GameObject.Find("Canvas").GetComponent<RectTransform>();
            m_CanvasGroup = gameObject.GetComponent<CanvasGroup>();
        }

        private void OnDestroy()
        {
            if (m_IsInitialized && m_Card != null)
                m_Card.CardAnalysedEvent -= OnCardAnalysed;
        }

        public void Initialize(Card card, RectTransform tableRoot, CardSlotGrid defaultCardSlotGrid)
        {
            m_Card = card;
            m_TableRoot = tableRoot;
            m_DefaultCardSlotGrid = defaultCardSlotGrid;

            m_Card.CardAnalysedEvent += OnCardAnalysed;

            //For editor purposes only
            gameObject.name = "Card: " + m_Card.CardDefinition.Name;

            //Initialize the arrows
            for (int i = 0; i < m_Arrows.Count; ++i)
            {
                m_Arrows[i].gameObject.SetActive(false);
            }

            for (int i = 0; i < m_Card.CardDefinition.Arrows.Count; ++i)
            {
                CardArrow cardArrow = m_Card.CardDefinition.Arrows[i];
                
                //Get the correct ID (we can have up to 3 arrows in each direction!
                int id = (int)cardArrow.Direction * 3;
                for (int j = 0; j < 3; ++j)
                {
                    if (m_Arrows[id].gameObject.activeSelf)
                    {
                        ++id;
                    }
                }

                //Activate and set color
                m_Arrows[id].gameObject.SetActive(true);
                m_Arrows[id].SetColor(cardArrow.Color);
            }
            
            FaceUp(false);

            m_IsInitialized = true;
        }

        private void Select()
        {
            m_CanvasGroup.blocksRaycasts = false;
            m_DraggedCard = this;

            //Parent ourselves to the table, so we always render on top!
            SetParent(m_TableRoot, 1.5f);
            SetVisualCardSlot(null);

            //Hide all the glow
            foreach (VisualArrow visualArrow in m_Arrows)
            {
                visualArrow.ShowGlow(false);
            }

            FaceUp(true);
        }

        private void Deselect()
        {
            m_CanvasGroup.blocksRaycasts = true;
            m_DraggedCard = null;

            if (m_VisualCardSlot == null)
            {
                if (m_LastVisualCardSlot == null)
                {
                    SetVisualCardSlot(m_DefaultCardSlotGrid.FirstEmptySlot());
                }
                else
                {
                    SetVisualCardSlot(m_LastVisualCardSlot);
                }

                if (m_VisualCardSlot == null)
                {
                    Debug.LogError("This card has nowhere to default to!");
                    return;
                }
            }

            SetParent(m_VisualCardSlot.transform, 1.0f);
            FaceUp(m_VisualCardSlot.FaceUp);
        }

        public void SetVisualCardSlot(VisualCardSlot visualCardSlot)
        {
            m_LastVisualCardSlot = m_VisualCardSlot;
            m_VisualCardSlot = visualCardSlot;

            if (m_VisualCardSlot != null)
            {
                m_Card.SetCardSlot(visualCardSlot.CardSlot);
                m_VisualCardSlot.UpdateCardSlot();
            }
            else
            {
                m_Card.SetCardSlot(null);
                if (m_LastVisualCardSlot != null)
                    m_LastVisualCardSlot.UpdateCardSlot();
            }
        }

        private void SetParent(Transform parent, float scale)
        {
            transform.SetParent(parent);
            transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
            transform.localScale = new Vector3(scale, scale, scale);

            UpdateRotation();
        }

        private void Rotate()
        {
            m_Card.Rotate();
            UpdateRotation();

            m_VisualCardSlot.UpdateCardSlot();
        }

        private void UpdateRotation()
        {
            Quaternion quat = Quaternion.identity;
            if (m_Card.IsRotated)
            {
                quat = Quaternion.Euler(new Vector3(0.0f, 0.0f, 180.0f));
            }

            transform.localRotation = quat;
        }

        private void FaceUp(bool value)
        {
            m_FaceUp = value;

            if (m_FaceUp)
            {
                //Add the title & description

                //Set the image
                m_Image.sprite = m_Card.CardDefinition.FrontSprite;
            }
            else
            {
                m_Image.sprite = m_Card.CardDefinition.BackSprite;
            }

            //Draw the arrows
            for (int i = 0; i < m_Arrows.Count; ++i)
            {
                m_Arrows[i].Hide(value);
            }
        }

        //----------------
        // INTERFACES
        //----------------

        //IPointerDownHandler
        public void OnPointerDown(PointerEventData eventData)
        {
            //We click the card
            //Debug.Log("Card clicked");
        }

        //IPointerUpHandler
        public void OnPointerUp(PointerEventData eventData)
        {
            //Only rotate when we were not dragging around
            if (m_DraggedCard == this)
                return;

            Rotate();
        }

        //IBeginDragHandler
        public void OnBeginDrag(PointerEventData eventData)
        {
            Select();
        }

        //IDragHandler
        public void OnDrag(PointerEventData eventData)
        {
            Vector3 newPoint = GetConvertedPosition();
            transform.position = newPoint;
        }

        //IEndDragHandler
        public void OnEndDrag(PointerEventData eventData)
        {
            Deselect();
        }

        //Events
        private void OnCardAnalysed(bool state, Direction dir)
        {
            //Make the arrows glow or not depending on if they are in a link or not!
            if (m_Card.IsRotated)
            {
                dir = Table.RotateDir(dir);
            }

            int id = (int)dir * 3;

            for (int j = 0; j < 3; ++j)
            {
                if (m_Arrows[id + j].gameObject.activeSelf)
                {
                    m_Arrows[id + j].ShowGlow(state);
                }
            }
        }

        private Vector3 GetConvertedPosition()
        {
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(m_CanvasTransform, Input.mousePosition, Camera.main, out localPoint);
            return m_CanvasTransform.TransformPoint(localPoint);
        }
    }
}
