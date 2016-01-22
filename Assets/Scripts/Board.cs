using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ArrowCardGame
{
    public class AnalyseResult
    {
        private List<CardSlot> m_InvolvedCardSlots;
        private int[] m_NumArrows;

        public AnalyseResult()
        {
            m_InvolvedCardSlots = new List<CardSlot>();
            m_NumArrows = new int[3]; //3 colors
        }

        //Mutators
        public void AddInvolvedCardSlot(CardSlot cardSlot)
        {
            m_InvolvedCardSlots.Add(cardSlot);
        }

        public void AddArrows(int amount, CardColor cardColor)
        {
            int cardColorID = (int)cardColor;
            m_NumArrows[cardColorID] += amount;
        }

        //Accesors
        public bool IsCardSlotInvolved(CardSlot cardSlot)
        {
            return m_InvolvedCardSlots.Contains(cardSlot);
        }

        public int GetNumberOfArrows(CardColor cardColor)
        {
            int cardColorID = (int)cardColor;
            return m_NumArrows[cardColorID];
        }

        public int TotalArrows()
        {
            int totalArrows = 0;
            totalArrows += GetNumberOfArrows(CardColor.Red);
            totalArrows += GetNumberOfArrows(CardColor.Green);
            totalArrows += GetNumberOfArrows(CardColor.Blue);

            return totalArrows;
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

            Debug.Log("Red: " + m_BestAnalyseResult.GetNumberOfArrows(CardColor.Red) +
                      " Green: " + m_BestAnalyseResult.GetNumberOfArrows(CardColor.Green) +
                      " Blue: " + m_BestAnalyseResult.GetNumberOfArrows(CardColor.Blue));
        }

        public void Resolve()
        {

        }

        public bool IsCardSlotInChain(CardSlot cardSlot)
        {
            return m_BestAnalyseResult.IsCardSlotInvolved(cardSlot);
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
