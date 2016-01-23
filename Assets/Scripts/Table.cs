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
            m_AIController = new AIController(m_Board.Board, m_OpponentHand.Deck, m_RedDeck.Deck, m_GreenDeck.Deck, m_BlueDeck.Deck);
            StartGame();
        }

        public void StartGame()
        {
            //Draw a starting hand
            m_PlayerHand.Deck.AddCard(m_RedDeck.Deck.DrawCard());
            m_PlayerHand.Deck.AddCard(m_GreenDeck.Deck.DrawCard());
            m_PlayerHand.Deck.AddCard(m_BlueDeck.Deck.DrawCard());

            m_OpponentHand.Deck.AddCard(m_RedDeck.Deck.DrawCard());
            m_OpponentHand.Deck.AddCard(m_GreenDeck.Deck.DrawCard());
            m_OpponentHand.Deck.AddCard(m_BlueDeck.Deck.DrawCard());
        }

        public void StartTurn(bool player)
        {

        }

        public void EndTurn()
        {
            m_Board.Resolve();
            StartCoroutine(AIPlayRoutine());
        }

        private IEnumerator AIPlayRoutine() 
        {
            //Disable all the decks & the player's hand.
            m_PlayerHand.AllowClicks(false);

            m_RedDeck.AllowClicks(false);
            m_GreenDeck.AllowClicks(false);
            m_BlueDeck.AllowClicks(false);
            

            //Give the player a bit of time to see what happened (resolve)
            yield return new WaitForSeconds(0.5f);

            m_AIController.DrawCard();
            m_AIController.CalculateMove();

            //Give the player a bit of time to see what card the AI drew
            yield return new WaitForSeconds(1.0f);

            m_AIController.PlayMove();
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
