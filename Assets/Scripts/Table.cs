using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ArrowCardGame
{
    //Functions as the GameManager. But I like the name "Table" in this case
    public delegate void VoidPlayerTypePlayerTypeDelegate(PlayerType playerType1, PlayerType playerType2);
    public delegate void VoidIntPlayerTypeDelegate(int i, PlayerType playerType);

    public enum TurnPhaseType
    {
        Draw = 0,
        PlayCard = 1,
        PlayedCard = 2,
        Resolve = 3
    }

    public class Table : Singleton<Table>
    {
        public static int DIR_NUM = 8;

        [SerializeField]
        private List<Player> m_Players;
        public List<Player> Players
        {
            get { return m_Players; }
        }

        private int m_CurrentPlayerID = 0;
        public int CurrentPlayerID
        {
            get { return m_CurrentPlayerID; }
        }

        [SerializeField]
        private VisualBoard m_Board;
        public VisualBoard Board
        {
            get { return m_Board; }
        }

        [SerializeField]
        private VisualDeck m_DiscardPile;

        //We only store this for the GetVisualCardSlot function
        [SerializeField]
        private List<VisualDeck> m_Hands;
        public List<VisualDeck> Hands
        {
            get { return m_Hands; }
        }

        [SerializeField]
        private List<VisualDeck> m_Decks;
        public List<VisualDeck> Decks
        {
            get { return m_Decks; }
        }

        private TurnPhase m_CurrentTurnPhase;
        private List<TurnPhase> m_TurnPhases;

        //Events

        //Game
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

        //Turns
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

            m_TurnPhases = new List<TurnPhase>();

            m_TurnPhases.Add(new DrawPhase(this));
            m_TurnPhases.Add(new PlayCardPhase(this));
            m_TurnPhases.Add(new PlayedCardState(this));
            m_TurnPhases.Add(new ResolvePhase(this));
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

            ChangeTurnPhase(TurnPhaseType.Draw);
        }

        public void EndDrawingPhase()
        {
            ChangeTurnPhase(TurnPhaseType.PlayCard);
        }

        public void PlayedCard(bool state)
        {
            if (state)
            {
                ChangeTurnPhase(TurnPhaseType.PlayedCard);
            }
            else
            {
                ChangeTurnPhase(TurnPhaseType.PlayCard);
            }
        }

        public void EndTurn()
        {
            ChangeTurnPhase(TurnPhaseType.Resolve);

            StopAllCoroutines();
            StartCoroutine(ResolveRoutine());
        }

        private IEnumerator ResolveRoutine()
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

            //Give us some time so the effects can go off
            yield return new WaitForSeconds(0.5f);

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

        public TurnPhase GetState(TurnPhaseType type)
        {
            return m_TurnPhases[(int)type];
        }

        private void ChangeTurnPhase(TurnPhaseType type)
        {
            if (m_CurrentTurnPhase == m_TurnPhases[(int)type])
                return;

            if (m_CurrentTurnPhase != null)
                m_CurrentTurnPhase.ExitPhase();

            m_CurrentTurnPhase = m_TurnPhases[(int)type];
            m_CurrentTurnPhase.EnterPhase();
        }

        public PlayerType GetCurrentPlayerType()
        {
            return m_Players[m_CurrentPlayerID].PlayerType;
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

        public int GetNextPlayerID()
        {
            return (m_CurrentPlayerID + 1) % m_Players.Count;
        }

        private int GetNextPlayerID(int playerID)
        {
            return (playerID + 1) % m_Players.Count;
        }
    }

    //Base class instead of interface as interfaces don't do well with property fiels.
    //This resulted in a lot more code duplication than expected. Therefore the TurnPhase class.
    public class TurnPhase
    {
        protected Table m_Table;

        //Events
        private VoidDelegate m_EnterPhaseEvent;
        public VoidDelegate EnterPhaseEvent
        {
            get { return m_EnterPhaseEvent; }
            set { m_EnterPhaseEvent = value; }
        }

        private VoidDelegate m_ExitPhaseEvent;
        public VoidDelegate ExitPhaseEvent
        {
            get { return m_ExitPhaseEvent; }
            set { m_ExitPhaseEvent = value; }
        }

        public TurnPhase(Table table)
        {
            m_Table = table;
        }

        public virtual void EnterPhase()
        {
            if (m_EnterPhaseEvent != null)
                m_EnterPhaseEvent();
        }

        public virtual void ExitPhase()
        {
            if (m_ExitPhaseEvent != null)
                m_ExitPhaseEvent();
        }
    }

    public class DrawPhase : TurnPhase
    {
        public DrawPhase(Table table) : base(table) {}

        public override void EnterPhase()
        {
            base.EnterPhase();

            //If the current player is AI. Lock everything!
            if (m_Table.GetCurrentPlayerType() == PlayerType.AI)
            {
                foreach (VisualDeck visualDeck in m_Table.Decks) { visualDeck.Lock(true); }
                foreach (VisualDeck visualDeck in m_Table.Hands) { visualDeck.Lock(true); }

                //Lock the entire board (otherwise the player can quickly take cards of the AI
                m_Table.Board.Lock(true);
            }
            else
            {
                //Unlock all the decks
                foreach (VisualDeck visualDeck in m_Table.Decks)
                {
                    visualDeck.Lock(false);
                }

                //Lock all the cards in our hand
                m_Table.Hands[m_Table.CurrentPlayerID].LockUsedSlots();

                //Lock the opponents hand
                m_Table.Hands[m_Table.GetNextPlayerID()].Lock(true);

                //Lock part of the board
                m_Table.Board.LockUsedSlots();
            }
        }

        public override void ExitPhase()
        {
            base.ExitPhase();

            //Lock all the decks
            for (int i = 0; i < m_Table.Decks.Count; ++i)
            {
                m_Table.Decks[i].Lock(true);
            }
        }
    }

    public class PlayCardPhase : TurnPhase
    {
        public PlayCardPhase(Table table) : base(table) {}

        public override void EnterPhase()
        {
            base.EnterPhase();

            //Unlock our hand
            if (m_Table.Players[m_Table.CurrentPlayerID].PlayerType == PlayerType.Human)
            {
                m_Table.Hands[m_Table.CurrentPlayerID].Lock(false);
            }
        }
    }

    public class PlayedCardState : TurnPhase 
    {
        public PlayedCardState(Table table) : base(table) { }

        public override void EnterPhase()
        {
            base.EnterPhase();

            //Lock the hand
            if (m_Table.Players[m_Table.CurrentPlayerID].PlayerType == PlayerType.Human)
            {
                m_Table.Hands[m_Table.CurrentPlayerID].Lock(true);
            }
        }
    }

    public class ResolvePhase : TurnPhase
    {
        public ResolvePhase(Table table) : base(table) {}
    }
}
