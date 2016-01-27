using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ArrowCardGame
{
    public enum PlayerType
    {
        Human = 0,
        AI = 1
    }

    public class Player : MonoBehaviour
    {
        [SerializeField]
        private DamageableObject m_DamageableObject;
        public DamageableObject DamageableObject
        {
            get { return m_DamageableObject; }
            set { m_DamageableObject = value; }
        }

        [SerializeField]
        private PlayerType m_PlayerType = PlayerType.Human;
        public PlayerType PlayerType
        {
            get { return m_PlayerType; }
            set { m_PlayerType = value; }
        }

        private VisualBoard m_Board;
        private VisualDeck m_Hand;
        private List<VisualDeck> m_Decks;

        private bool m_IsPlaying = false;
        public bool IsPlaying
        {
            get { return m_IsPlaying; }
            set { m_IsPlaying = value; }
        }

        private AIController m_AIController;

        private void Start()
        {
            Table table = Table.Instance;
            table.StartGameEvent += OnStartGame;
            table.StartTurnEvent += OnStartTurn;
        }

        public void Initialize(VisualBoard board, VisualDeck hand, List<VisualDeck> decks)
        {
            m_Board = board;
            m_Hand = hand;
            m_Decks = decks;

            List<Deck> deckData = new List<Deck>();
            foreach (VisualDeck visualDeck in m_Decks)
            {
                visualDeck.DrawCardEvent += OnCardDraw;
                deckData.Add(visualDeck.Deck);
            }

            m_AIController = new AIController(m_Board.Board, m_Hand.Deck, deckData);
        }

        private void OnDestroy()
        {
            if (m_Decks == null)
                return;

            foreach (VisualDeck visualDeck in m_Decks)
            {
                if (visualDeck != null)
                    visualDeck.DrawCardEvent -= OnCardDraw;
            }
        }


        private void OnStartGame(PlayerType playerType1, PlayerType playerType2)
        {
            //Draw a card from each deck
            foreach (VisualDeck visualDeck in m_Decks)
            {
                m_Hand.Deck.AddCard(visualDeck.Deck.DrawCard());
            }
        }

        private void OnStartTurn(int playerID, PlayerType playerType)
        {
            if (!m_IsPlaying)
                return;

            if (m_PlayerType == PlayerType.AI)
            {
                AutoPlay();
            }
        }

        private void OnCardDraw()
        {
            //If it's our turn
            if (!m_IsPlaying)
                return;

            if (m_Hand.NumberOfCards() >= 3)
            {
                Table.Instance.EndDrawingPhase();
            }
        }


        private void AutoPlay()
        {
            StartCoroutine(AIRoutine());
        }

        private IEnumerator AIRoutine()
        {
            //Give the player a bit of time to see what happened (resolve)
            yield return new WaitForSeconds(0.5f);

            m_AIController.DrawCard();
            Table.Instance.EndDrawingPhase();

            m_AIController.CalculateMove();

            //Give the player a bit of time to see what card the AI drew
            yield return new WaitForSeconds(1.0f);

            m_AIController.PlayMove();

            yield return new WaitForSeconds(1.0f);
            Table.Instance.EndTurn();
        }
    }
}