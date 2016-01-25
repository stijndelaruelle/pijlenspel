using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ArrowCardGame
{
    //Functions as the GameManager. But I like the name "Table" in this case
    public delegate void VoidPlayerTypePlayerTypeDelegate(PlayerType playerType1, PlayerType playerType2);
    public delegate void VoidIntPlayerTypeDelegate(int i, PlayerType playerType);

    public class Table : Singleton<Table>
    {
        public static int DIR_NUM = 8;

        [SerializeField]
        private List<Player> m_Players;
        private int m_CurrentPlayerID = 0;

        [SerializeField]
        private VisualBoard m_Board;

        [SerializeField]
        private VisualDeck m_DiscardPile;

        //We only store this for the GetVisualCardSlot function
        [SerializeField]
        private List<VisualDeck> m_Hands;

        [SerializeField]
        private List<VisualDeck> m_Decks;

        //Events
        private VoidPlayerTypePlayerTypeDelegate m_StartGameEvent;
        public VoidPlayerTypePlayerTypeDelegate StartGameEvent
        {
            get { return m_StartGameEvent; }
            set { m_StartGameEvent = value; }
        }

        private VoidIntPlayerTypeDelegate m_EndGameEvent;
        public VoidIntPlayerTypeDelegate EndGameEvent
        {
            get { return m_EndGameEvent; }
            set { m_EndGameEvent = value; }
        }

        private VoidIntPlayerTypeDelegate m_StartTurnEvent;
        public VoidIntPlayerTypeDelegate StartTurnEvent
        {
            get { return m_StartTurnEvent; }
            set { m_StartTurnEvent = value; }
        }

        private VoidDelegate m_EndTurnEvent;
        public VoidDelegate EndTurnEvent
        {
            get { return m_EndTurnEvent; }
            set { m_EndTurnEvent = value; }
        }

        protected override void Awake()
        {
            base.Awake();
            if (m_Players.Count < 2)
            {
                Debug.LogError("The table does not have 2 players!");
            }
        }

        private void Start()
        {
            //Initialize our players
            for (int i = 0; i < m_Players.Count; ++i)
            {
                m_Players[i].Initialize(m_Board, m_Hands[i], m_Decks);
            }

            //StartGame(PlayerType.Human, PlayerType.AI);
        }

        public void StartGame()
        {
            StartGame(m_Players[0].PlayerType, m_Players[1].PlayerType);
        }

        public void StartGame(PlayerType playerType1, PlayerType playerType2)
        {
            m_Players[0].PlayerType = playerType1;
            m_Players[1].PlayerType = playerType2;

            foreach (VisualDeck visualDeck in m_Decks)
            {
                visualDeck.Deck.ResetDeck();
            }

            if (m_StartGameEvent != null)
                m_StartGameEvent(playerType1, playerType2);

            m_CurrentPlayerID = m_Players.Count - 1;
            StartTurn();
        }

        private void EndGame(int winnerPlayerID)
        {
            if (m_EndGameEvent != null)
                m_EndGameEvent(winnerPlayerID, m_Players[winnerPlayerID].PlayerType);
        }

        public void StartTurn()
        {
            m_CurrentPlayerID = GetNextPlayerID(m_CurrentPlayerID);
            m_Players[m_CurrentPlayerID].IsPlaying = true;

            if (m_StartTurnEvent != null)
                m_StartTurnEvent(m_CurrentPlayerID, m_Players[m_CurrentPlayerID].PlayerType);
        }

        public void EndTurn()
        {
            ArrowResult result = m_Board.Resolve(m_DiscardPile);

            if (result.redArrows != 0 || result.blueArrows != 0)
            {
                //Attack
                if (result.redArrows > result.blueArrows)
                {
                    int nextPlayerID = GetNextPlayerID(m_CurrentPlayerID);
                    m_Players[nextPlayerID].DamageableObject.ModifyHealth(-result.redArrows);
                }
                
                //Heal
                if (result.redArrows < result.blueArrows)
                {
                    m_Players[m_CurrentPlayerID].DamageableObject.ModifyHealth(result.blueArrows);
                }

                //Decide
                if (result.redArrows == result.blueArrows)
                {
                    //For now ALWAYS ATTACK
                    int nextPlayerID = GetNextPlayerID(m_CurrentPlayerID);
                    m_Players[nextPlayerID].DamageableObject.ModifyHealth(-result.redArrows);
                }
            }

            if (m_EndTurnEvent != null)
                m_EndTurnEvent();

            m_Players[m_CurrentPlayerID].IsPlaying = false;

            if (!CheckForGameOver())
                StartTurn();
        }

        private bool CheckForGameOver()
        {
            for (int i = 0; i < m_Players.Count; ++i)
            {
                if (m_Players[i].DamageableObject.Health <= 0)
                {
                    int winnerPlayerId = GetNextPlayerID(i);
                    EndGame(winnerPlayerId);
                    return true;
                }
            }

            return false;
        }

        public void AllowCardDrawing(bool state)
        {
            for (int i = 0; i < m_Decks.Count; ++i)
            {
                m_Decks[i].AllowClicks(state);
            }
        }


        //Utility, shouldn't be here?
        public VisualCardSlot GetVisualCardSlot(CardSlot cardSlot)
        {
            VisualCardSlot visualCardSlot;

            //This is ugly, but we'll leave it like this for now. As it's experimental
            visualCardSlot = m_Board.GetVisualCardSlot(cardSlot);
            if (visualCardSlot != null) return visualCardSlot;

            visualCardSlot = m_DiscardPile.GetVisualCardSlot(cardSlot);
            if (visualCardSlot != null) return visualCardSlot;

            foreach (VisualDeck visualDeck in m_Hands)
            {
                visualCardSlot = visualDeck.GetVisualCardSlot(cardSlot);
                if (visualCardSlot != null) return visualCardSlot;
            }

            foreach (VisualDeck visualDeck in m_Decks)
            {
                visualCardSlot = visualDeck.GetVisualCardSlot(cardSlot);
                if (visualCardSlot != null) return visualCardSlot;
            }

            return null;
        }

        public static Direction RotateDir(Direction dir)
        {
            Direction rotatedDir = (Direction)(((int)dir + 4) % Table.DIR_NUM);
            return rotatedDir;
        }

        private int GetNextPlayerID(int playerID)
        {
            return (playerID + 1) % m_Players.Count;
        }
    }
}
