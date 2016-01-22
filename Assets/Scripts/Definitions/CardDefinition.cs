using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace ArrowCardGame
{
    public enum CardColor
    {
        Red = 0,
        Green = 1,
        Blue = 2
    }

    public enum Direction
    {
        TopCenter = 0,
        TopRight = 1, 
        MiddleRight = 2, 
        BottomRight = 3,
        BottomCenter = 4,
        BottomLeft = 5,
        MiddleLeft = 6,
        TopLeft = 7,
    }

    [System.Serializable]
    public class CardArrow
    {
        [SerializeField]
        private Direction m_Direction;
        public Direction Direction
        {
            get { return m_Direction; }
        }

        [SerializeField]
        private CardColor m_Color;
        public CardColor Color
        {
            get { return m_Color; }
        }

        [SerializeField]
        private bool m_IsConditional;
        public bool IsConditional
        {
            get { return m_IsConditional; }
        }
    }

    public class CardDefinition : ScriptableObject
    {
        //Basic info
        [SerializeField]
        private string m_Name;
        public string Name
        {
            get { return m_Name; }
        }

        [SerializeField]
        private string m_Description;
        public string Description
        {
            get { return m_Description; }
        }

        [SerializeField]
        private CardColor m_Color;
        public CardColor Color
        {
            get { return m_Color; }
        }

        [SerializeField]
        private int m_Tier;
        public int Tier
        {
            get { return m_Tier; }
        }

        [SerializeField]
        private Sprite m_FrontSprite;
        public Sprite FrontSprite
        {
            get { return m_FrontSprite; }
        }

        [SerializeField]
        private Sprite m_BackSprite;
        public Sprite BackSprite
        {
            get { return m_BackSprite; }
        }

        [SerializeField]
        private List<CardArrow> m_Arrows = new List<CardArrow>();
        public List<CardArrow> Arrows
        {
            get { return m_Arrows; }
        }

        //Effects: Grants extra (conditional turns)
    }
}
