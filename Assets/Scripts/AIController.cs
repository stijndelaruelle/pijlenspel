using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ArrowCardGame
{
    public class AIController
    {
        private class AIAnalyseResult
        {
            private Card m_Card;
            public Card Card
            {
                get { return m_Card; }
            }

            private CardSlot m_CardSlot;
            public CardSlot CardSlot
            {
                get { return m_CardSlot; }
            }

            private bool m_Rotated;
            public bool Rotated
            {
                get { return m_Rotated; }
            }

            private AnalyseResult m_AnalyseResult;
            public AnalyseResult AnalyseResult
            {
                get { return m_AnalyseResult; }
            }

            public AIAnalyseResult(Card card, CardSlot cardSlot, bool rotated, AnalyseResult analyseResult)
            {
                m_Card = card;
                m_CardSlot = cardSlot;
                m_Rotated = rotated;
                m_AnalyseResult = analyseResult;
            }
        }

        private Board m_Board;
        private Deck m_Hand;

        public AIController(Board board, Deck hand)
        {
            m_Board = board;
            m_Hand = hand;
        }

        public void Process()
        {
            if (m_Hand.Cards.Count == 0)
                return;

            List<AIAnalyseResult> analyseResults = new List<AIAnalyseResult>();

            //For every card in my hand
            foreach (Card card in m_Hand.Cards)
            {
                //Place it on every GridSlot from the board
                foreach (CardSlot cardSlot in m_Board.CardSlots)
                {
                    if (cardSlot.Card == null)
                    {
                        CardSlot oldCardSlot = card.CardSlot;

                        //Both regular
                        card.CardSlot = cardSlot;
                        analyseResults.Add(new AIAnalyseResult(card, cardSlot, cardSlot.Card.IsRotated, m_Board.Analyse()));

                        //And rotated
                        card.Rotate();
                        analyseResults.Add(new AIAnalyseResult(card, cardSlot, cardSlot.Card.IsRotated, m_Board.Analyse()));

                        //Reset
                        card.Rotate();
                        card.CardSlot = oldCardSlot;
                    }
                }
            }

            //Now we've literally analysed every possible move.
            AIAnalyseResult preferredMove = null;

            //Get the best result
            for (int i = 0; i < analyseResults.Count; ++i)
            {
                //Update result
                if (preferredMove == null)                                                           
                {
                    preferredMove = analyseResults[i];
                }
                else if (analyseResults[i].AnalyseResult.TotalArrows() > preferredMove.AnalyseResult.TotalArrows())
                {
                    preferredMove = analyseResults[i];
                }
            }

            //We're unable to make a chain! Just do the worst possible move then.
            //if (preferredMove.AnalyseResult.TotalArrows() < 3)
            //{
            //    //Get the worst Result
            //    for (int i = 0; i < analyseResults.Count; ++i)
            //    {
            //        //Update result
            //        if (preferredMove == null)
            //        {
            //            preferredMove = analyseResults[i];
            //        }
            //        else if (analyseResults[i].AnalyseResult.TotalArrows() < preferredMove.AnalyseResult.TotalArrows())
            //        {
            //            preferredMove = analyseResults[i];
            //        }
            //    }
            //}

            //Play that move!
            preferredMove.Card.Rotate(preferredMove.Rotated);
            preferredMove.Card.CardSlot = preferredMove.CardSlot;
        }
    }
}
