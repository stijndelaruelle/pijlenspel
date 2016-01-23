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
        private Deck m_RedDeck;
        private Deck m_GreenDeck;
        private Deck m_BlueDeck;

        private AIAnalyseResult m_PreferredMove;

        public AIController(Board board, Deck hand, Deck redDeck, Deck greenDeck, Deck blueDeck)
        {
            m_Board = board;
            m_Hand = hand;
            m_RedDeck = redDeck;
            m_GreenDeck = greenDeck;
            m_BlueDeck = blueDeck;
        }

        public void DrawCard()
        {
            //Don't care about empty decks!

            float rand = Random.RandomRange(0.0f, 100.0f);
            int colorId = (int)rand / 33;
            CardColor cardColor = (CardColor)colorId;

            Card card = null;

            switch (cardColor)
            {
                case CardColor.Red:
                    card = m_RedDeck.DrawCard();
                    break;
                    
                case CardColor.Green:
                    card = m_GreenDeck.DrawCard();
                    break;

                case CardColor.Blue:
                    card = m_BlueDeck.DrawCard();
                    break;

                default:
                    break;
            }
            
            if (card != null)
                m_Hand.AddCard(card);
        }

        public void CalculateMove()
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

            if (analyseResults.Count == 0)
            {
                Debug.LogWarning("There were no possiblities for the AI to consider, DO SOMETHING!");
                return;
            }

            //Now we've literally analysed every possible move.
            m_PreferredMove = null;

            //Get the best result
            for (int i = 0; i < analyseResults.Count; ++i)
            {
                //Update result
                if (m_PreferredMove == null)                                                           
                {
                    m_PreferredMove = analyseResults[i];
                }
                else if (analyseResults[i].AnalyseResult.TotalArrows() > m_PreferredMove.AnalyseResult.TotalArrows())
                {
                    m_PreferredMove = analyseResults[i];
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
        }

        public void PlayMove()
        {
            //Play that move!
            m_PreferredMove.Card.Rotate(m_PreferredMove.Rotated);
            m_PreferredMove.Card.CardSlot = m_PreferredMove.CardSlot;
            m_Board.Analyse(); //Analyse to update everything

            m_Hand.RemoveCard(m_PreferredMove.Card);
        }
    }
}
