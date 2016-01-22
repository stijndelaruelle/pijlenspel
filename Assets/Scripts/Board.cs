using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ArrowCardGame
{
    public class AnalyseResult
    {
        private struct SpecificCardArrow
        {
            public SpecificCardArrow(CardSlot cardSlot, CardArrow cardArrow)
            {
                this.cardSlot = cardSlot;
                this.cardArrow = cardArrow;
            }

            public CardSlot cardSlot;
            public CardArrow cardArrow;
        }

        private List<SpecificCardArrow> m_SpecificCardArrow;

        public AnalyseResult()
        {
            m_SpecificCardArrow = new List<SpecificCardArrow>();
        }

        //Mutators
        public void AddArrows(CardSlot cardSlot, CardArrow cardArrow)
        {
            m_SpecificCardArrow.Add(new SpecificCardArrow(cardSlot, cardArrow));
        }

        //Accesors
        public bool IsCardSlotInvolved(CardSlot cardSlot)
        {
            for (int i = 0; i < m_SpecificCardArrow.Count; ++i)
            {
                if (m_SpecificCardArrow[i].cardSlot == cardSlot)
                    return true;
            }

            return false;
        }

        public List<CardArrow> GetLinkedArrows(CardSlot cardSlot)
        {
            List<CardArrow> involvedArrows = new List<CardArrow>();

            for (int i = 0; i < m_SpecificCardArrow.Count; ++i)
            {
                if (m_SpecificCardArrow[i].cardSlot == cardSlot)
                {
                    involvedArrows.Add(m_SpecificCardArrow[i].cardArrow);
                }
            }

            return involvedArrows;
        }

        public int TotalArrows()
        {
            return m_SpecificCardArrow.Count;
        }
    }

    public class Board : MonoBehaviour
    {
        [SerializeField]
        private int m_Width;

        [SerializeField]
        private int m_Height;

        [SerializeField]
        private List<VisualCardSlot> m_VisualCardSlots;

        private List<AnalyseResult> m_AnalyseResults;
        private AnalyseResult m_BestAnalyseResult;
        private AnalyseResult m_WorstAnalyseResult;

        private void Start()
        {
            foreach (VisualCardSlot visualCardSlot in m_VisualCardSlots)
            {
                visualCardSlot.CardSlotUpdatedEvent += OnCardSlotUpdated;
            }

            m_AnalyseResults = new List<AnalyseResult>();
            InitializeGrid();
        }

        private void OnDestroy()
        {
            if (m_VisualCardSlots == null)
                return;

            foreach (VisualCardSlot visualCardSlot in m_VisualCardSlots)
            {
                visualCardSlot.CardSlotUpdatedEvent -= OnCardSlotUpdated;
            }
        }

        private void InitializeGrid()
        {
            //Set the neighbours
            for (int i = 0; i < m_VisualCardSlots.Count; ++i)
            {
                CardSlot cardSlot = m_VisualCardSlots[i].CardSlot;

                //Top left
                if ((i / m_Width > 0) &&
                    (i % m_Width > 0))
                {
                    cardSlot.SetNeightbour(Direction.TopLeft, m_VisualCardSlots[i - m_Width - 1].CardSlot);
                }

                //Top center
                if (i / m_Width > 0)
                {
                    cardSlot.SetNeightbour(Direction.TopCenter, m_VisualCardSlots[i - m_Width].CardSlot);
                }

                //Top right
                if ((i / m_Width > 0) &&
                    (i % m_Width < (m_Width - 1)))
                {
                    cardSlot.SetNeightbour(Direction.TopRight, m_VisualCardSlots[i - m_Width + 1].CardSlot);
                }

                //Middle left
                if (i % m_Width > 0)
                {
                    cardSlot.SetNeightbour(Direction.MiddleLeft, m_VisualCardSlots[i - 1].CardSlot);
                }

                //Middle right
                if (i % m_Width < (m_Width - 1))
                {
                    cardSlot.SetNeightbour(Direction.MiddleRight, m_VisualCardSlots[i + 1].CardSlot);
                }

                //Bottom left
                if ((i / m_Width < (m_Height - 1)) &&
                    (i % m_Width > 0))
                {
                    cardSlot.SetNeightbour(Direction.BottomLeft, m_VisualCardSlots[i + m_Width - 1].CardSlot);
                }

                //Bottom center
                if (i / m_Width < (m_Height - 1))
                {
                    cardSlot.SetNeightbour(Direction.BottomCenter, m_VisualCardSlots[i + m_Width].CardSlot);
                }

                //Bottom right
                if ((i / m_Width < (m_Height - 1)) &&
                    (i % m_Width < (m_Width - 1)))
                {
                    cardSlot.SetNeightbour(Direction.BottomRight, m_VisualCardSlots[i + m_Width + 1].CardSlot);
                }

            }
        }

        public void Analyse()
        {
            m_AnalyseResults.Clear();

            for (int i = 0; i < m_VisualCardSlots.Count; ++i)
            {
                //Check if this slot was already analysed, if so. Skip it! (OPTIMALISATION, DO LATER)

                //Analyse this slot
                AnalyseResult analyseResult = new AnalyseResult();
                m_VisualCardSlots[i].CardSlot.Analyse(analyseResult);

                m_AnalyseResults.Add(analyseResult);
            }

            m_BestAnalyseResult = CalculateBestResult();
            m_WorstAnalyseResult = CalculateWorstResult();
        }

        public bool IsCardSlotInChain(CardSlot cardSlot)
        {
            return m_BestAnalyseResult.IsCardSlotInvolved(cardSlot);
        }

        public List<CardArrow> GetLinkedArrows(CardSlot cardSlot)
        {
            for (int i = 0; i < m_AnalyseResults.Count; ++i)
            {
                List<CardArrow> linkedArrows = m_AnalyseResults[i].GetLinkedArrows(cardSlot);

                if (linkedArrows.Count > 0)
                    return linkedArrows;
            }

            return new List<CardArrow>();
        }

        private AnalyseResult CalculateBestResult()
        {
            AnalyseResult bestResult = null;

            for (int i = 0; i < m_AnalyseResults.Count; ++i)
            {
                //Update result
                if (bestResult == null)
                {
                    bestResult = m_AnalyseResults[i];
                }
                else if (m_AnalyseResults[i].TotalArrows() > bestResult.TotalArrows())
                {
                    bestResult = m_AnalyseResults[i];
                }
            }

            return bestResult;
        }

        private AnalyseResult CalculateWorstResult()
        {
            AnalyseResult worstResult = null;

            for (int i = 0; i < m_AnalyseResults.Count; ++i)
            {
                //Update result
                if (worstResult == null)
                {
                    worstResult = m_AnalyseResults[i];
                }
                else if (m_AnalyseResults[i].TotalArrows() < worstResult.TotalArrows())
                {
                    worstResult = m_AnalyseResults[i];
                }
            }

            return worstResult;
        }

        private void OnCardSlotUpdated()
        {
            Analyse();
        }
    }
}
