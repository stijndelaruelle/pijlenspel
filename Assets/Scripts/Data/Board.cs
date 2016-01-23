using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace ArrowCardGame
{
    public class AnalyseResult
    {
        private List<CardSlot> m_InvolvedCardSlots;
        public List<CardSlot> InvolvedCardSlots
        {
            get { return m_InvolvedCardSlots; }
        }

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

    public class Board
    {
        private int m_Width;
        private int m_Height;
        private List<CardSlot> m_CardSlots;
        public List<CardSlot> CardSlots
        {
            get { return m_CardSlots; }
        }

        //Analyse results
        private List<AnalyseResult> m_AnalyseResults;
        private AnalyseResult m_BestAnalyseResult;

        public Board(int width, int height, List<CardSlot> cardSlots)
        {
            m_Width = width;
            m_Height = height;
            m_CardSlots = cardSlots;

            InitializeGrid();

            m_AnalyseResults = new List<AnalyseResult>();
        }

        private void InitializeGrid()
        {
            //Set the neighbours
            for (int i = 0; i < m_CardSlots.Count; ++i)
            {
                CardSlot currentCardSlot = m_CardSlots[i];

                //Top left
                if ((i / m_Width > 0) &&
                    (i % m_Width > 0))
                {
                    currentCardSlot.SetNeightbour(Direction.TopLeft, m_CardSlots[i - m_Width - 1]);
                }

                //Top center
                if (i / m_Width > 0)
                {
                    currentCardSlot.SetNeightbour(Direction.TopCenter, m_CardSlots[i - m_Width]);
                }

                //Top right
                if ((i / m_Width > 0) &&
                    (i % m_Width < (m_Width - 1)))
                {
                    currentCardSlot.SetNeightbour(Direction.TopRight, m_CardSlots[i - m_Width + 1]);
                }

                //Middle left
                if (i % m_Width > 0)
                {
                    currentCardSlot.SetNeightbour(Direction.MiddleLeft, m_CardSlots[i - 1]);
                }

                //Middle right
                if (i % m_Width < (m_Width - 1))
                {
                    currentCardSlot.SetNeightbour(Direction.MiddleRight, m_CardSlots[i + 1]);
                }

                //Bottom left
                if ((i / m_Width < (m_Height - 1)) &&
                    (i % m_Width > 0))
                {
                    currentCardSlot.SetNeightbour(Direction.BottomLeft, m_CardSlots[i + m_Width - 1]);
                }

                //Bottom center
                if (i / m_Width < (m_Height - 1))
                {
                    currentCardSlot.SetNeightbour(Direction.BottomCenter, m_CardSlots[i + m_Width]);
                }

                //Bottom right
                if ((i / m_Width < (m_Height - 1)) &&
                    (i % m_Width < (m_Width - 1)))
                {
                    currentCardSlot.SetNeightbour(Direction.BottomRight, m_CardSlots[i + m_Width + 1]);
                }

            }
        }

        public AnalyseResult Analyse()
        {
            m_AnalyseResults.Clear();

            for (int i = 0; i < m_CardSlots.Count; ++i)
            {
                //Check if this slot was already analysed, if so. Skip it! (OPTIMALISATION, DO LATER)

                //Analyse this slot
                AnalyseResult analyseResult = new AnalyseResult();
                m_CardSlots[i].Analyse(analyseResult);

                m_AnalyseResults.Add(analyseResult);
            }

            m_BestAnalyseResult = CalculateBestResult();

            //Debug.Log("Red: " + m_BestAnalyseResult.GetNumberOfArrows(CardColor.Red) +
            //          " Green: " + m_BestAnalyseResult.GetNumberOfArrows(CardColor.Green) +
            //          " Blue: " + m_BestAnalyseResult.GetNumberOfArrows(CardColor.Blue));

            return m_BestAnalyseResult;
        }

        public void Resolve()
        {
            foreach (CardSlot cardSlot in m_BestAnalyseResult.InvolvedCardSlots)
            {
                //The card on all these slots should move to the discard pile

            }
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
    }
}
