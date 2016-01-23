using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

namespace ArrowCardGame
{
    public class VisualArrow : MonoBehaviour
    {
        [SerializeField]
        private Image m_ArrowImage;

        [SerializeField]
        private GameObject m_GlowObject;

        private void Awake()
        {
            m_GlowObject.SetActive(false);
        }

        public void Hide(bool state)
        {
            m_ArrowImage.enabled = state;
            
            if (state == false)
                ShowGlow(false);
        }

        public void SetColor(CardColor cardColor)
        {
            switch (cardColor)
            {
                case CardColor.Red:
                    m_ArrowImage.color = Color.red;
                    break;

                case CardColor.Green:
                    m_ArrowImage.color = Color.green;
                    break;

                case CardColor.Blue:
                    m_ArrowImage.color = Color.cyan;
                    break;

                default:
                    break;
            }
        }

        public void ShowGlow(bool state)
        {
            m_GlowObject.SetActive(state);
        }
    }
}
