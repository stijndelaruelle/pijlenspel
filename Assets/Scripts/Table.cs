using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ArrowCardGame
{
    //Functions as the GameManager. But I like the name "Table" in this case
    public class Table : Singleton<Table>
    {
        public static int DIR_NUM = 8;

        [SerializeField]
        private VisualBoard m_Board;

        [SerializeField]
        private VisualDeck m_PlayerHand;

        [SerializeField]
        private VisualDeck m_OpponentHand;

        [SerializeField]
        private VisualDeck m_RedDeck;

        [SerializeField]
        private VisualDeck m_GreenDeck;

        [SerializeField]
        private VisualDeck m_BlueDeck;

        private AIController m_AIController;

        public void Start()
        {
            m_AIController = new AIController(m_Board.Board, m_OpponentHand.Deck);
        }

        public void EndTurn()
        {
            m_Board.Resolve();
        }

        public void AIMove()
        {
            m_AIController.Process();
        }

        public VisualCardSlot GetVisualCardSlot(CardSlot cardSlot)
        {
            VisualCardSlot visualCardSlot;

            //This is ugly, but we'll leave it like this for now. As it's experimental
            visualCardSlot = m_Board.GetVisualCardSlot(cardSlot);
            if (visualCardSlot != null) return visualCardSlot;

            visualCardSlot = m_PlayerHand.GetVisualCardSlot(cardSlot);
            if (visualCardSlot != null) return visualCardSlot;

            visualCardSlot = m_OpponentHand.GetVisualCardSlot(cardSlot);
            if (visualCardSlot != null) return visualCardSlot;

            visualCardSlot = m_RedDeck.GetVisualCardSlot(cardSlot);
            if (visualCardSlot != null) return visualCardSlot;

            visualCardSlot = m_GreenDeck.GetVisualCardSlot(cardSlot);
            if (visualCardSlot != null) return visualCardSlot;

            visualCardSlot = m_BlueDeck.GetVisualCardSlot(cardSlot);
            if (visualCardSlot != null) return visualCardSlot;

            return null;
        }

        public static Direction RotateDir(Direction dir)
        {
            Direction rotatedDir = (Direction)(((int)dir + 4) % Table.DIR_NUM);
            return rotatedDir;
        }
    }
}
