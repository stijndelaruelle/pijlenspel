using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace ArrowCardGame
{
    public class CardSlot
    {
        //Bolow a diagram of how the neighbour ID's work
        // 0   1  2
        //    --
        // 3 |  | 4 
        //    -- 
        // 5   6  7

        private CardSlot[] m_Neighbours;

        private Card m_Card;
        public Card Card
        {
            get { return m_Card; }
            set
            {
                if (m_Card != value)
                {
                    m_Card = value;

                    if (m_Card != null)
                        m_Card.CardSlot = this;
                }
            }
        }

        public CardSlot()
        {
            m_Neighbours = new CardSlot[Table.DIR_NUM];
        }

        public bool IsEmpty()
        {
            return (m_Card == null);
        }

        public void Analyse(AnalyseResult analyseResult)
        {
            //Feels a bit weird that the main analyse logic is in the cardslot class, rethink later.
            analyseResult.AddInvolvedCardSlot(this);

            if (m_Card == null)
                return;

            foreach (CardArrow arrow in m_Card.GetArrows())
            {
                analyseResult.AddArrows(1, arrow.Color);
            }

            //For every direction
            for (int i = 0; i < Table.DIR_NUM; ++i)
            {
                Direction dir = (Direction)i;
                m_Card.CardAnalysed(false, dir);

                //Do we have an at least 1 arrow in this direction?
                if (m_Card.HasArrow(dir))
                {
                    //Do we have a neighbour in this direction?
                    CardSlot neighbour = GetNeighbour(dir);
                    if (neighbour != null)
                    {
                        //Does our neighbour have an arrow in the opposite direction?
                        Direction rotatedDir = Table.RotateDir(dir);

                        if (neighbour.Card != null && neighbour.Card.HasArrow(rotatedDir))
                        {
                            //Yes! We have a "link". Add all our arrows in this direction.
                            m_Card.CardAnalysed(true, dir);
                            foreach (CardArrow arrow in m_Card.GetArrows((Direction)i))
                            {
                                analyseResult.AddInvolvedArrows(1, arrow.Color);
                            }

                            //If our neighbour isn't analysed, analyse him
                            if (!analyseResult.IsCardSlotInvolved(neighbour))
                            {
                                neighbour.Analyse(analyseResult);
                            }
                        }
                    }
                }
            }
        }

        public void SetNeightbour(Direction dir, CardSlot cardSlot)
        {
            int intDir = (int)dir;

            if (intDir >= m_Neighbours.Length) return;
            m_Neighbours[intDir] = cardSlot;
        }

        public CardSlot GetNeighbour(Direction dir)
        {
            int intDir = (int)dir;

            if (intDir >= m_Neighbours.Length) return null;
            return m_Neighbours[intDir];
        }
    }
}