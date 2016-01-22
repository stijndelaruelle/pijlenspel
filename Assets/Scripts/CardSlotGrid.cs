using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ArrowCardGame
{
    public class CardSlotGrid : MonoBehaviour
    {
        [SerializeField]
        private List<VisualCardSlot> m_VisualCardSlots;

        public VisualCardSlot FirstEmptySlot()
        {
            foreach (VisualCardSlot visualCardSlot in m_VisualCardSlots)
            {
                if (visualCardSlot.IsEmpty())
                    return visualCardSlot;
            }

            return null;
        }
    }
}